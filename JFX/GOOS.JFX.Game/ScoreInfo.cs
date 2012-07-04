using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GOOS.JFX.Game
{
	/// <summary>
	/// Represents score info
	/// </summary>
	[Serializable]
	public class ScoreInfo : IComparable<ScoreInfo>, ISerializable
	{
		#region Compare Delegate
		
		public ScoreCompareDelegate CompareHandler;		

		#endregion

		#region IComparable Members

		public int CompareTo(ScoreInfo obj)
		{
			return CompareHandler(this, obj,SortOrder);
		}

		#endregion

		#region Members

		private string mSortOrder;
		private int mBasicScore;
		private string mBasicName;
		private Dictionary<string,IComparable> mExtendedValues;

		#endregion

		#region Properties

		public string SortOrder
		{
			get { return mSortOrder; }
			set { mSortOrder = value; }
		}

		public Dictionary<string, IComparable> ExtendedValues
		{
			get { return mExtendedValues; }
			set { mExtendedValues = value; }
		}

		public string BasicName
		{
			get { return mBasicName; }
			set { mBasicName = value; }
		}

		public int BasicScore
		{
			get { return mBasicScore; }
			set { mBasicScore = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Deserialization constructor
		/// </summary>
		protected ScoreInfo(SerializationInfo info, StreamingContext context)
		{
			this.mSortOrder = (string)info.GetValue("sortorder", typeof(string));
			this.mBasicName = (string)info.GetValue("basicname", typeof(string));
			this.mSortOrder = (string)info.GetValue("sortorder", typeof(string));			
			this.CompareHandler = (ScoreCompareDelegate)info.GetValue("comparehandler", typeof(ScoreCompareDelegate));
			
			string[] keys = (string[])info.GetValue("keys", typeof(string[]));
			object[] values = (object[])info.GetValue("values", typeof(object[]));

			ExtendedValues = new Dictionary<string, IComparable>();
			for (int i = 0; i < keys.Length; i++)			
				ExtendedValues.Add(keys[i],(IComparable)values[i]);			
		}

		/// <summary>
		/// By default use the basic comparison handler
		/// </summary>
		private ScoreInfo(bool useDefaultComparer, ScoreCompareDelegate target, string order)
		{
			ExtendedValues = new Dictionary<string, IComparable>();
			SortOrder = order;

			if (useDefaultComparer)
				CompareHandler += new ScoreCompareDelegate(DefaultScoreCompare);
			else if(target != null)
				CompareHandler += new ScoreCompareDelegate(target);
		}

		#endregion

		#region Factory

		/// <summary>
		/// Return a new ScoreInfo using the default comparer
		/// </summary>	
		public static ScoreInfo CreateNewDefault(string order)
		{
			return new ScoreInfo(true, null,order);
		}
		public static ScoreInfo CreateNewDefault()
		{
			return new ScoreInfo(true, null, string.Empty);
		}

		/// <summary>
		/// Return a new ScoreInfo using the provided compare method.
		/// </summary>
		public static ScoreInfo CreateNewWithComparer(ScoreCompareDelegate target,string order)
		{
			return new ScoreInfo(false, target,order);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Set a score property
		/// </summary>
		/// <param name="key">The name of the score property</param>
		/// <param name="value">The value to set it to.</param>
		public void Set(string key, IComparable value)
		{
			ExtendedValues[key] = value;
		}

		#endregion

		#region Delegate Methods

		public int DefaultScoreCompare(ScoreInfo s1, ScoreInfo s2,string order)
		{
			//Compare basic scores first
			int compare = s1.BasicScore.CompareTo(s2.BasicScore);

			if (compare == 0)			
			{
				string[] sort1, sort2;					
				sort1 = order.Split("|".ToCharArray());

				for (int i = 0; i < sort1.Length; i++)
				{
					if (sort1[i].Contains(";"))
					{
						sort2 = sort1[i].Split(";".ToCharArray());
						if (sort2[1].ToUpper() == "ASC")
							compare = s1.ExtendedValues[sort2[0]].CompareTo(s2.ExtendedValues[sort2[0]]);
						if (sort2[1].ToUpper() == "DESC")
							compare = s2.ExtendedValues[sort2[0]].CompareTo(s1.ExtendedValues[sort2[0]]);
					}
					else					
						compare = s2.ExtendedValues[sort1[i]].CompareTo(s1.ExtendedValues[sort1[i]]);

					if (compare != 0)
						return compare;
				}
			}			
			return compare;
		}

		#endregion

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("sortorder", this.SortOrder);
			info.AddValue("basicscore", this.BasicScore);
			info.AddValue("basicname", this.BasicName);			
			info.AddValue("comparehandler", this.CompareHandler);

			//Extended
			string[] str  = new string[ExtendedValues.Values.Count];			
			object[] obj = new object[ExtendedValues.Values.Count];
			int index = 0;

			foreach (string key in ExtendedValues.Keys)
			{
				str[index] = key;
				obj[index] = (object)ExtendedValues[key];
				index++;
			}

			info.AddValue("size", index);
			info.AddValue("keys", str);
			info.AddValue("values", obj);
		}

		#endregion
	}

	#region Delegate Definition

	public delegate int ScoreCompareDelegate(ScoreInfo s1, ScoreInfo s2,string order);

	#endregion
}
