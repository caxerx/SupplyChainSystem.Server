using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SupplyChainSystem.Server.Hub;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MapRequestController : Controller
    {
        private readonly ProcedurementContext _dbContext;
        private readonly IHubContext<NotificationHub> _hubContext;

        public MapRequestController(ProcedurementContext dbContext, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }


        [HttpPost]
        [Authorize]
        public SupplyResponse Post()
        {
            var requestQueue = _dbContext.Request.Include(p => p.RequestItem).ThenInclude(p => p.VirtualItem)
                .ThenInclude(p => p.VirtualIdMap).ThenInclude(p => p.Item)
                .Where(p => p.RequestStatus == RequestStatus.WaitingForProcess ||
                            p.RequestStatus == RequestStatus.Failed)
                .Select(p => p);


            var mapStatus = new List<dynamic>();

            int desp = 0;

            foreach (var request in requestQueue)
            {
                Console.WriteLine($"Processing request {request.RequestId}");
                _dbContext.Request.SingleOrDefault(p => p.RequestId == request.RequestId).RequestStatus =
                    RequestStatus.Processing;
                _dbContext.SaveChanges();
                //find all items in request
                var vItems = request.RequestItem.Select(p => p.VirtualItem).ToList();

                //search for active blanket
                var blanketAgreements = _dbContext.Agreement.Where(p =>
                        p.AgreementType == AgreementType.Blanket && DateTime.Now < p.ExpiryDate &&
                        DateTime.Now > p.StartDate)
                    .Select(p => p).Include(p => p.Supplier)
                    .Include(p => p.BlanketPurchaseAgreementDetails)
                    .Include(p => p.BlanketPurchaseAgreementLines).ThenInclude(p => p.Item);

                var itemMatchedAgreement = new List<Agreement>();

                //search any agreement that contains all items in Request status
                foreach (var agreement in blanketAgreements)
                {
                    Console.WriteLine(
                        $"mapping {request.RequestId}, testing for {agreement.AgreementId}:contains item");
                    var agreementVItems = agreement.BlanketPurchaseAgreementLines.Select(p =>
                        (p.Item.VirtualIdMap ?? new List<VirtualIdMap>()).Select(q => q.VirtualItem)).Aggregate(
                        (ta, i) =>
                        {
                            var ls = new List<VirtualItem>();
                            ls.AddRange(ta);
                            ls.AddRange(i);
                            return ls;
                        });

                    if (vItems.All(p => agreementVItems.Contains(p)))
                    {
                        Console.WriteLine(
                            $"mapping {request.RequestId}, {agreement.AgreementId} contains all needed item");
                        itemMatchedAgreement.Add(agreement);
                    }
                }

                var matchedAgreementList = new List<Agreement>();
                //find promised qty & min qty
                foreach (var agreement in itemMatchedAgreement)
                {
                    Console.WriteLine(
                        $"mapping {request.RequestId}, testing for {agreement.AgreementId}:qty and price");
                    var amount = 0d;
                    var lines = agreement.BlanketPurchaseAgreementLines;
                    var matchedLines = new List<dynamic>();
                    foreach (var requestItem in request.RequestItem)
                    {
                        var vItem = requestItem.VirtualItem;
                        var matchedLine = lines.SingleOrDefault(line =>
                        {
                            if ((line.Item.VirtualIdMap ?? new List<VirtualIdMap>()).Select(p => p.VirtualItem)
                                .Contains(vItem) &&
                                requestItem.Quantity >= line.MinimumQuantity &&
                                line.UsedQuantity + requestItem.Quantity < line.PromisedQuantity)
                            {
                                Console.WriteLine(
                                    $"mapping {request.RequestId}, a line in agreement {agreement.AgreementId} match");
                                amount += line.Price * requestItem.Quantity;
                                return true;
                            }

                            return false;
                        });
                        if (matchedLine == null)
                        {
                            Console.WriteLine(
                                $"mapping {request.RequestId}, can't find a line to match request item in agreement {agreement.AgreementId}");
                            amount = 0d;
                            matchedLines.Clear();
                            break;
                        }

                        matchedLines.Add(new {requestItem, matchedLine});
                    }

                    var details = agreement.BlanketPurchaseAgreementDetails;
                    if (matchedLines.Any() && details.AmountUsed + amount <= details.AmountAgreed)
                    {
                        matchedAgreementList.Add(agreement);
                        Console.WriteLine($"mapping {request.RequestId}, {agreement.AgreementId} match");
                    }
                    else
                    {
                        Console.WriteLine($"mapping {request.RequestId}, {agreement.AgreementId} not match");
                        Console.WriteLine(
                            $"{details.AmountUsed},{amount},{details.AmountUsed + amount},{details.AmountAgreed}");
                    }
                }


                if (matchedAgreementList.Any())
                {
                    var selectedAgreement = matchedAgreementList
                        .OrderBy(p => p.ExpiryDate).First();
                    Console.WriteLine($"Adding request map {request.RequestId}");
                    var _dbRequestMap = _dbContext.RequestMap.Add(new RequestMap
                    {
                        AgreementId = selectedAgreement.AgreementId,
                        MapType = MapType.BPA,
                        RequestId = request.RequestId
                    });

                    _dbContext.SaveChanges();

                    var dbRelease = _dbContext.BlanketRelease.Add(new BlanketRelease
                    {
                        CreateTime = DateTime.Now,
                        RequestId = request.RequestId,
                        AgreementId = selectedAgreement.AgreementId,
                    });

                    _dbContext.SaveChanges();

                    var agreementLines = selectedAgreement.BlanketPurchaseAgreementLines;

                    var price = 0d;
                    foreach (var requestItem in request.RequestItem)
                    {
                        var itemLine = agreementLines.SingleOrDefault(p =>
                            p.Item.VirtualIdMap.SingleOrDefault(q => q.VirtualItemId == requestItem.VirtualItemId) !=
                            null);


                        var _dbLine = _dbContext.BlanketPurchaseAgreementLine.SingleOrDefault(p =>
                            p.AgreementId == selectedAgreement.AgreementId && p.ItemId == itemLine.ItemId);
                        _dbLine.UsedQuantity += requestItem.Quantity;
                        _dbContext.SaveChanges();

                        _dbContext.BlanketReleaseLine.Add(new BlanketReleaseLine
                        {
                            OrderId = dbRelease.Entity.OrderId,
                            ItemId = itemLine.ItemId,
                            Price = itemLine.Price,
                            Quantity = requestItem.Quantity
                        });
                        _dbContext.SaveChanges();
                        price += itemLine.Price * requestItem.Quantity;
                    }

                    _dbContext.BlanketPurchaseAgreementDetails.SingleOrDefault(p =>
                        p.AgreementId == selectedAgreement.AgreementId).AmountUsed += price;
                    _dbContext.SaveChanges();


                    _dbContext.Request.SingleOrDefault(p => p.RequestId == request.RequestId).RequestStatus =
                        RequestStatus.Ordered;
                    _dbContext.SaveChanges();

                    mapStatus.Add(new
                    {
                        RequestId = request.RequestId,
                        MapStatus = new
                        {
                            Success = true,
                            Type = "BPA"
                        },
                        RequestMap = _dbRequestMap.Entity,
                        Supplier = selectedAgreement.Supplier
                    });

                    continue;
                }


                //Warehouse

                var warehouseStock = _dbContext.Stock.Include(p => p.StockItem)
                    .FirstOrDefault(p => p.StockType == StockType.WarehouseStock);

                if (warehouseStock != null)
                {
                    Console.WriteLine(
                        $"Testing warehouse {request.RequestId}, warehouse stock is {warehouseStock.StockId}");
                    var warehouseStockItem = warehouseStock.StockItem;
                    //var stockItem = warehouseStock.Select(p => p.VirtualItem);

                    if (request.RequestItem.All(
                        p => warehouseStockItem.FirstOrDefault(q =>
                                 p.VirtualItemId == q.VirtualItemId && q.Quantity >= p.Quantity) != null))
                    {
                        Console.WriteLine($"Adding warehouse map {request.RequestId}");

                        var despatchInstruction = _dbContext.DespatchInstruction.Add(new DespatchInstruction
                        {
                            CreateTime = DateTime.Now,
                            RequestId = request.RequestId,
                            DespatchInstructionStatus = 0
                        });

                        foreach (var requestItem in request.RequestItem)
                        {
                            warehouseStockItem.SingleOrDefault(p => p.VirtualItemId == requestItem.VirtualItemId)
                                    .Quantity -=
                                requestItem.Quantity;
                            _dbContext.SaveChanges();
                        }


                        var _dbRequestMap = _dbContext.RequestMap.Add(new RequestMap
                        {
                            MapType = MapType.Warehouse,
                            RequestId = request.RequestId,
                            DespatchInstructionId = despatchInstruction.Entity.DespatchInstructionId
                        });

                        _dbContext.SaveChanges();


                        _dbContext.Request.SingleOrDefault(p => p.RequestId == request.RequestId).RequestStatus =
                            RequestStatus.WaitingForDespatch;
                        _dbContext.SaveChanges();


                        mapStatus.Add(new
                        {
                            RequestId = request.RequestId,
                            MapStatus = new
                            {
                                Success = true,
                                Type = "Warehouse"
                            },
                            RequestMap = _dbRequestMap.Entity
                        });

                        desp++;

                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Warehouse stock not found, skipped");
                }

                //failed
                {
                    Console.WriteLine($"No match for request {request.RequestId}");
                    _dbContext.Request.SingleOrDefault(p => p.RequestId == request.RequestId).RequestStatus =
                        RequestStatus.Failed;
                    _dbContext.SaveChanges();

                    var contractAgreements = _dbContext.Agreement.Where(p =>
                            p.AgreementType == AgreementType.Contract && DateTime.Now < p.ExpiryDate &&
                            DateTime.Now > p.StartDate)
                        .Include(p => p.ContractPurchaseAgreementDetails)
                        .Include(p => p.ContractPurchaseAgreementLines).ThenInclude(p => p.Item)
                        .ThenInclude(p => p.VirtualIdMap).ThenInclude(p => p.VirtualItem)
                        .Include(p => p.Supplier)
                        .Select(p => p);

                    var matchedContract = new List<Agreement>();

                    foreach (var agreement in contractAgreements)
                    {
                        var contractVirtualItem = agreement.ContractPurchaseAgreementLines
                            .Select(p => p.Item.VirtualIdMap).Select(p => p.Select(q => q.VirtualItem))
                            .Aggregate((all, t) =>
                            {
                                var its = new List<VirtualItem>();
                                its.AddRange(all);
                                its.AddRange(t);
                                return its;
                            });
                        if (request.RequestItem.All(p => contractVirtualItem.Contains(p.VirtualItem)))
                        {
                            matchedContract.Add(agreement);
                        }
                    }

                    mapStatus.Add(new
                    {
                        RequestId = request.RequestId,
                        MapStatus = new
                        {
                            Success = false
                        },
                        RequestMap = new RequestMap {RequestId = request.RequestId, Request = request},
                        ContractMatched = matchedContract
                    });
                }
            }

            var lastStatus = new
            {
                LastSuccess = DateTime.Now,
                Maps = mapStatus
            };


            _dbContext.DataCache.Add(new DataCache
            {
                RemovalTime = DateTime.Now.AddDays(1),
                CacheTime = DateTime.Now,
                CacheType = "RequestMap",
                Content = JsonConvert.SerializeObject(lastStatus, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })
            });

            _dbContext.SaveChanges();

            if (desp > 0)
            {
                _hubContext.Clients.All.SendAsync("ReceiveMessage", " Warehouse",
                    $"{desp} new despatch instruction(s) come.");
            }

            return SupplyResponse.Ok(lastStatus);
        }


        [HttpPost("{id}")]
        [Authorize]
        public SupplyResponse Post(int id, [FromBody] RequestMap map)
        {
            var _dbAgreement = _dbContext.Agreement
                .Include(p => p.ContractPurchaseAgreementDetails)
                .Include(p => p.ContractPurchaseAgreementLines).ThenInclude(p => p.Item).SingleOrDefault(p =>
                    p.AgreementId == map.AgreementId &&
                    p.AgreementType == AgreementType.Contract && DateTime.Now < p.ExpiryDate &&
                    DateTime.Now > p.StartDate);

            if (_dbAgreement == null)
            {
                return SupplyResponse.NotFound("Active Agreement", map.AgreementId + "");
            }

            var request = _dbContext.Request.Include(p => p.RequestItem).ThenInclude(p => p.VirtualItem)
                .ThenInclude(p => p.VirtualIdMap).ThenInclude(p => p.Item)
                .SingleOrDefault(p =>
                    p.RequestId == id && (p.RequestStatus == RequestStatus.WaitingForProcess ||
                                          p.RequestStatus == RequestStatus.Failed));
            if (request == null)
            {
                return SupplyResponse.NotFound("Unprocessed Request", map.RequestId + "");
            }


            var contractVirtualItem = _dbAgreement.ContractPurchaseAgreementLines
                .Select(p => p.Item.VirtualIdMap).Select(p => p?.Select(q => q.VirtualItem))
                .Aggregate((all, t) =>
                {
                    var its = new List<VirtualItem>();
                    its.AddRange(all);
                    its.AddRange(t);
                    return its;
                });

            if ((request.RequestItem ?? new List<RequestItem>()).All(p => contractVirtualItem.Contains(p.VirtualItem)))
            {
                var _dbRequestMap = _dbContext.RequestMap.Add(new RequestMap
                {
                    RequestId = request.RequestId,
                    AgreementId = _dbAgreement.AgreementId,
                    MapType = MapType.Contract
                });
                _dbContext.SaveChanges();
                var order = new StandardPurchaseOrder
                {
                    CreateTime = DateTime.Now,
                    AgreementId = _dbAgreement.AgreementId,
                    RequestId = request.RequestId
                };

                var _dbOrder = _dbContext.StandardPurchaseOrder.Add(order);
                _dbContext.SaveChanges();

                var idmaps = _dbAgreement.ContractPurchaseAgreementLines
                    .Select(p => p.Item.VirtualIdMap).Select(p => p)
                    .Aggregate((all, t) =>
                    {
                        var its = new List<VirtualIdMap>();
                        its.AddRange(all);
                        its.AddRange(t);
                        return its;
                    });


                foreach (var requestItem in request.RequestItem ?? new List<RequestItem>())
                {
                    foreach (var idmap in idmaps)
                    {
                        if (idmap.VirtualItemId == requestItem.VirtualItemId)
                        {
                            _dbContext.StandardPurchaseOrderLine.Add(new StandardPurchaseOrderLine
                            {
                                OrderId = _dbOrder.Entity.OrderId,
                                ItemId = idmap.ItemId,
                                Quantity = requestItem.Quantity,
                            });
                            _dbContext.SaveChanges();
                            break;
                        }
                    }
                }

                request.RequestStatus = RequestStatus.Ordered;
                _dbContext.SaveChanges();

                return SupplyResponse.Ok(new
                {
                    RequestId = request.RequestId,
                    MapStatus = new
                    {
                        Success = true,
                        Type = "Contract"
                    },
                    RequestMap = _dbRequestMap.Entity
                });
            }

            return SupplyResponse.Fail("Agreement Not Match", "The selected agreement can't match this request.");
        }


        [HttpGet]
        [Authorize]
        public SupplyResponse Get()
        {
            var res = _dbContext.DataCache.OrderByDescending(p => p.CacheTime)
                .FirstOrDefault(p => p.CacheType == "RequestMap");
            return res == null
                ? SupplyResponse.Fail("First Request Mapping",
                    "System never run a request mapping before. To start request map manually, click start button.")
                : SupplyResponse.Ok(JsonConvert.DeserializeObject(res.Content));
        }
    }
}