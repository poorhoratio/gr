namespace TakeHome.Console.Models
{
	public enum ChangeType
    {
		Corrupted = 1,
		Missing = 2,
		New = 3
    }

	public class AccountChange
	{
		public ChangeType ChangeType { get; init; }
		public string Id { get; init; }
		public string OldName { get; init; }
		public string OldEmail { get; init; }
		public string NewName { get; init; }
		public string NewEmail { get; init; }

		public string Csv
		{
			get
			{
				var csv = ChangeType switch
				{
					ChangeType.Corrupted => $"{ChangeType},{Id},{OldName},{OldEmail},{NewName},{NewEmail}",
					ChangeType.Missing => $"{ChangeType},{Id},{OldName},{OldEmail},,",
					ChangeType.New => $"{ChangeType},{Id},,,{NewName},{NewEmail}",
					_ => "",
				};
				return csv;
			}
		}
	}
}
