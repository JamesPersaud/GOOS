using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GOOS.JFX.Game
{	
	/// <summary>
	/// A class to represent the hi-score table.
	/// </summary>	
	[Serializable]
	public class HiScoreTable : ISerializable
	{
		#region Members	

		private List<ScoreInfo> mScores;
		private string mColumns;
		private string mSortOrder;

		#endregion

		#region Properties

		/// <summary>
		/// The columns - including which to display and in what order.
		/// </summary>
		public string Columns
		{
			get { return mColumns; }
			set { mColumns = value; }
		}

		/// <summary>
		/// A string describing the sort order.
		/// </summary>
		public string SortOrder
		{
			get { return mSortOrder; }
			set { mSortOrder = value; }
		}

		/// <summary>
		/// The list of scores
		/// </summary>
		public List<ScoreInfo> Scores
		{
			get { return mScores; }
			set { mScores = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Deserialization constructor
		/// </summary>
		protected HiScoreTable(SerializationInfo info, StreamingContext context)
		{
			this.mColumns = (string)info.GetValue("columns", typeof(string));
			this.mScores = (List<ScoreInfo>)info.GetValue("scores", typeof(List<ScoreInfo>));
			this.mSortOrder = (string)info.GetValue("sort", typeof(string));
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="s">The sort order</param>
		public HiScoreTable(string c, string s)
		{
			Scores = new List<ScoreInfo>();
			SortOrder = s;
			Columns = c;
		}

		#endregion

		#region Factory

		/// <summary>
		/// Load the hiscore table from a file
		/// </summary>
		/// <param name="path">the full file path</param>
		/// <returns>a new hiscore table object</returns>
		public static HiScoreTable LoadFromFile(string path)
		{
			if (File.Exists(path))
			{
				try
				{
					HiScoreTable t;
					Stream stream = File.Open(path, FileMode.Open);
					BinaryFormatter bFormatter = new BinaryFormatter();
					t = (HiScoreTable)bFormatter.Deserialize(stream);
					stream.Close();
					return t;
				}
				catch
				{
					return null;
				}
			}
			return null;
		}

		/// <summary>
		/// Load the hiscore table from a file or an empty one if the file is not found.
		/// </summary>
		/// <param name="path">the full file path</param>
		/// <returns>a new hiscore table object</returns>
		public static HiScoreTable LoadFromFile(string path, string columns, string sort)
		{
			if (File.Exists(path))
			{
				try
				{
					HiScoreTable t;
					Stream stream = File.Open(path, FileMode.Open);
					BinaryFormatter bFormatter = new BinaryFormatter();
					t = (HiScoreTable)bFormatter.Deserialize(stream);
					stream.Close();
					return t;
				}
				catch
				{
					HiScoreTable hi = new HiScoreTable(columns, sort);
					return hi;
				}
			}
			else
			{
				HiScoreTable hi = new HiScoreTable(columns, sort);
				return hi;
			}
		}

		/// <summary>
		/// Creates a new Hi score table with a given sort order
		/// </summary>
		/// <param name="sort">the sort order</param>
		/// <returns>A new hiscore table object</returns>
		public static HiScoreTable NewEmptyTable(string columns, string sort)
		{
			HiScoreTable hi = new HiScoreTable(columns, sort);
			return hi;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Save this hiscore table to a file
		/// </summary>
		/// <param name="path">the full file path</param>
		public void SaveToFile(string path)
		{
			Stream stream = File.Open(path, FileMode.Create);
			BinaryFormatter bFormatter = new BinaryFormatter();
			bFormatter.Serialize(stream, this);
			stream.Close();
		}

		/// <summary>
		/// Add a score to the table.
		/// </summary>		
		public void AddScore(ScoreInfo score)
		{
			score.SortOrder = this.SortOrder;
			Scores.Add(score);
		}

		/// <summary>
		/// Add a new score by its extended values - an array ordered as the columns
		/// </summary>
		/// <param name="values">An array of IComparable values in the same sequence as this.columns</param>
		public void AddScore(params IComparable[] values)
		{
			ScoreInfo score = ScoreInfo.CreateNewDefault(SortOrder);
			string[] keys = Columns.Split("|".ToCharArray());
			for (int i = 0; i < keys.Length; i++)			
				score.Set(keys[i], values[i]);			
			Scores.Add(score);
		}

		/// <summary>
		/// Add a new score by its extended values and basic attributes - an array ordered as the columns
		/// </summary>
		/// <param name="basicScore">Basic score</param>
		/// <param name="basicName">Basic name</param>
		/// <param name="values">An array of IComparable values in the same sequence as this.columns</param>
		public void AddScore(int basicScore,string basicName,params IComparable[] values)
		{
			ScoreInfo score = ScoreInfo.CreateNewDefault(SortOrder);
			score.BasicName = basicName;
			score.BasicScore = basicScore;
			string[] keys = Columns.Split("|".ToCharArray());
			for (int i = 0; i < keys.Length; i++)
				score.Set(keys[i], values[i]);
			Scores.Add(score);
		}

		/// <summary>
		/// Add a new score by its basic attributes - an array ordered as the columns
		/// </summary>
		/// <param name="basicScore">Basic score</param>
		/// <param name="basicName">Basic name</param>
		public void AddScore(int basicScore, string basicName)
		{
			ScoreInfo score = ScoreInfo.CreateNewDefault(SortOrder);
			score.BasicName = basicName;
			score.BasicScore = basicScore;			
			Scores.Add(score);
		}

		/// <summary>
		/// Sort this table.
		/// </summary>
		public void Sort()
		{
			Scores.Sort();
		}

		/// <summary>
		/// Get the top scores
		/// </summary>
		/// <param name="number">Number of scores to get</param>
		/// <returns>A list of scores.</returns>
		public List<ScoreInfo> GetTop(int number)
		{
			Sort();
			if (number > Scores.Count)
				number = Scores.Count;
			return Scores.GetRange(0, number);
		}

		#endregion

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("columns", mColumns);
			info.AddValue("sort", mSortOrder);
			info.AddValue("scores", mScores);			
		}

		#endregion
	}
}
