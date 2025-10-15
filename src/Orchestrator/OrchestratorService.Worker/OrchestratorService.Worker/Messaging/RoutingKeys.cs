﻿namespace OrchestratorService.Worker.Messaging;

public static class Rk
{
    // events (domain)
    public const string OrderCreated = "order.created";
    public const string InventoryStockReserved = "inventory.stock.reserved";
    public const string InventoryStockFailed = "inventory.stock.failed";
    public const string PaymentSucceeded = "payment.succeeded";
    public const string PaymentFailed = "payment.failed";
    public const string OrderConfirmed = "order.confirmed";
    public const string OrderFailed = "order.failed";

    // commands (orchestrator -> participants)
    public const string CmdInventoryReserve = "cmd.inventory.reserve";
    public const string CmdInventoryRelease = "cmd.inventory.release";
    public const string CmdPaymentRequest = "cmd.payment.request";
    public const string CmdPaymentCancel = "cmd.payment.cancel";
    public const string CmdOrderUpdateStatus = "cmd.order.update-status";
}
