using MediatR;
using System;
using InventoryService.Contracts.Inventory;

namespace InventoryService.Application.Inventory.GetInventoryById;

public sealed record GetInventoryByIdQuery(Guid Id) : IRequest<GetInventoryResponse?>; 