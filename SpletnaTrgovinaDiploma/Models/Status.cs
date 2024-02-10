namespace SpletnaTrgovinaDiploma.Models
{
    public enum OrderStatus
    {
        Processing, // V obdelavi
        WaitingForPayment, // Čaka plačilo
        Paid, // Plačano
        ReadyForPickupOrSent, //Pripravljeno za prevzem/Poslano
        Finished, // Zaključeno
        Cancelled // Preklicano
    }
}
