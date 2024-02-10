using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpletnaTrgovinaDiploma.Data.ViewModels;
using SpletnaTrgovinaDiploma.Models;

namespace SpletnaTrgovinaDiploma.Helpers
{
    public static class OrderStatusUtil
    {
        public static string GetOrderStatusText(this OrderStatus orderStatus)
        {
            var text = orderStatus switch
            {
                OrderStatus.Processing => "Processing",
                OrderStatus.WaitingForPayment => "Waiting for payment",
                OrderStatus.Paid => "Paid",
                OrderStatus.ReadyForPickupOrSent => "Sent",
                OrderStatus.Finished => "Finished",
                OrderStatus.Cancelled => "Cancelled",
                _ => throw new NotSupportedException()
            };

            return text;
        }

        public static SelectList GetDropdownValues(this OrderStatus orderStatus, PaymentOption paymentOption)
            => new(orderStatus.GetAvailableStatuses(paymentOption), "Value", "Text");

        static IEnumerable<SelectListItem> GetAvailableStatuses(this OrderStatus orderStatus,
            PaymentOption paymentOption)
        {
            return paymentOption switch
            {
                PaymentOption.CreditCard => orderStatus.GetCreditCardStatuses(),
                PaymentOption.FundsTransfer => orderStatus.GetFundsTransferStatuses(),
                PaymentOption.Cash => orderStatus.GetCashStatuses(),
                _ => throw new NotSupportedException()
            };
        }

        static IEnumerable<SelectListItem> GetCreditCardStatuses(this OrderStatus orderStatus)
        {
            if (orderStatus == OrderStatus.Processing)
                yield return OrderStatus.WaitingForPayment.GetSelectListItem();

            if (orderStatus == OrderStatus.WaitingForPayment)
                yield return OrderStatus.Paid.GetSelectListItem();

            if (orderStatus == OrderStatus.Paid)
                yield return OrderStatus.ReadyForPickupOrSent.GetSelectListItem();

            if (orderStatus == OrderStatus.ReadyForPickupOrSent)
                yield return OrderStatus.Finished.GetSelectListItem();

            if (orderStatus != OrderStatus.Finished && orderStatus != OrderStatus.Cancelled)
                yield return OrderStatus.Cancelled.GetSelectListItem();
        }

        static IEnumerable<SelectListItem> GetFundsTransferStatuses(this OrderStatus orderStatus)
        {
            if (orderStatus == OrderStatus.Processing)
                yield return OrderStatus.WaitingForPayment.GetSelectListItem();

            if (orderStatus == OrderStatus.WaitingForPayment)
                yield return OrderStatus.Paid.GetSelectListItem();

            if (orderStatus == OrderStatus.Paid)
                yield return OrderStatus.ReadyForPickupOrSent.GetSelectListItem();

            if (orderStatus == OrderStatus.ReadyForPickupOrSent)
                yield return OrderStatus.Finished.GetSelectListItem();

            if (orderStatus != OrderStatus.Finished && orderStatus != OrderStatus.Cancelled)
                yield return OrderStatus.Cancelled.GetSelectListItem();
        }

        static IEnumerable<SelectListItem> GetCashStatuses(this OrderStatus orderStatus)
        {
            if (orderStatus == OrderStatus.Processing)
                yield return OrderStatus.ReadyForPickupOrSent.GetSelectListItem();

            if (orderStatus == OrderStatus.ReadyForPickupOrSent)
                yield return OrderStatus.Finished.GetSelectListItem();

            if (orderStatus != OrderStatus.Finished && orderStatus != OrderStatus.Cancelled)
                yield return OrderStatus.Cancelled.GetSelectListItem();
        }

        static SelectListItem GetSelectListItem(this OrderStatus orderStatus)
            => new(orderStatus.GetOrderStatusText(), orderStatus.ToString());
    }
}