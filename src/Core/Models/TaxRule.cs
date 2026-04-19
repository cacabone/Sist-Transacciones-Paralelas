namespace BankSystem.Core.Models
{
	public class TaxRule
	{
		public const decimal TaxRate = 0.0015m; // 0.15%

		// Monto mínimo por canal para que aplique impuesto
		public const decimal CajeroMinAmount = 10000m;
		public const decimal AgenteBancarioMinAmount = 0m;    // siempre aplica
		public const decimal ViaElectronicaMinAmount = 0m;    // siempre aplica

		// Solo estos tipos de transacción generan impuesto por canal
		public static bool AppliesTax(TransactionChannel channel,
									   TransactionType type,
									   decimal amount)
		{
			return channel switch
			{
				TransactionChannel.Cajero =>
					type == TransactionType.Retiro && amount >= CajeroMinAmount,

				TransactionChannel.AgenteBancario =>
					type == TransactionType.Retiro ||
					type == TransactionType.Transferencia,

				TransactionChannel.ViaElectronica =>
					true, // siempre aplica sin importar tipo ni monto

				_ => false
			};
		}

		public static decimal Calculate(decimal amount) =>
			Math.Round(amount * TaxRate, 2);
	}
}