using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CustomAuth.Entitites;

public class TaskList
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int TaskID { get; set; } // Auto-generated identity and primary key

	public int UserID { get; set; } // Foreign key from UserAccount table

	public string Task_Name { get; set; } // varchar
	public string Task_Description { get; set; } // varchar
	public string Task_Status { get; set; } // nvarchar
	public int Task_Priority { get; set; } // int

	// Navigation property
	public UserAccount User { get; set; }
}
