using ErpDemo.Application.Common;
using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using ErpDemo.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Pedidos")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista pedidos com paginação e filtros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetAll(
        [FromQuery] Guid? customerId,
        [FromQuery] OrderStatus? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetPagedAsync(customerId, status, fromDate, toDate, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Busca pedido por ID com itens
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    /// <summary>
    /// Cria um novo pedido com itens
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        var order = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Atualiza status do pedido (Draft → Confirmed, Draft/Confirmed → Canceled)
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        var order = await _service.UpdateStatusAsync(id, dto);
        return Ok(order);
    }

    /// <summary>
    /// Adiciona um item ao pedido (somente Draft)
    /// </summary>
    [HttpPost("{orderId:guid}/items")]
    [ProducesResponseType(typeof(OrderItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItemDto>> AddItem(Guid orderId, [FromBody] CreateOrderItemDto dto)
    {
        var item = await _service.AddItemAsync(orderId, dto);
        return Created($"/api/orders/{orderId}/items/{item.Id}", item);
    }

    /// <summary>
    /// Atualiza um item do pedido (somente Draft)
    /// </summary>
    [HttpPut("{orderId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(OrderItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderItemDto>> UpdateItem(Guid orderId, Guid itemId, [FromBody] UpdateOrderItemDto dto)
    {
        var item = await _service.UpdateItemAsync(orderId, itemId, dto);
        return Ok(item);
    }

    /// <summary>
    /// Remove um item do pedido (somente Draft)
    /// </summary>
    [HttpDelete("{orderId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem(Guid orderId, Guid itemId)
    {
        await _service.RemoveItemAsync(orderId, itemId);
        return NoContent();
    }

    /// <summary>
    /// Exclui um pedido (somente Draft ou Canceled, Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
