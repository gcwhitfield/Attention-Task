using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class data_saver : MonoBehaviour {

	[SerializeField] bool _comprehension_enabled = true;
	[SerializeField] InputField ID;
	// [SerializeField] Dropdown grade;xx
	[SerializeField] Dropdown version;
	[SerializeField] InputField dob;
	[SerializeField] Dropdown fall_or_spring;
	// [SerializeField] Dropdown type_school;xx
	// [SerializeField] Dropdown sex;xx
	// [SerializeField] Dropdown ethnicity;xx
	[SerializeField] Dropdown adhd;
	[SerializeField] Dropdown adhdpres;
	[SerializeField] InputField Q1;
	[SerializeField] InputField Q2;
	[SerializeField] InputField Q3;
	[SerializeField] InputField Q4;
	[SerializeField] InputField Q5;
	[SerializeField] InputField Q6;
	static bool comprehension_enabled;

	// class for saving data to database
	public class StroopData
	{
		public float total_Time;
		public int search_Trials_Correct;
		public int search_Trials_Incorrect;
		public float search_Trial_Accuracy;
		public float average_Search_Trial_Reaction_Time;
		public int catch_Trials_Correct;
		public int catch_Trials_Incorrect;
		public float catch_Trial_Accuracy;
		public float average_Catch_Trial_Reaction_Time;
		public int total_Trials_Correct;
		public int total_Trials_Incorrect;
	}

	// this function is called right after the stroop task is complete and before loading
	// the next scene
	public StroopData CreateDatabaseObject(
		float total_Time,
		int search_Trials_Correct,
		int search_Trials_Incorrect,
		float search_Trial_Accuracy,
		float average_Search_Trial_Reaction_Time,
		int catch_Trials_Correct,
		int catch_Trials_Incorrect,
		float catch_Trial_Accuracy,
		float average_Catch_Trial_Reaction_Time,
		int total_Trials_Correct,
		int total_Trials_Incorrect
	)
	{
		StroopData result = new StroopData();
		result.total_Time = total_Time;
		result.search_Trials_Correct = search_Trials_Correct;
		result.search_Trials_Incorrect = search_Trials_Incorrect;
		result.search_Trial_Accuracy = search_Trial_Accuracy;
		result.average_Search_Trial_Reaction_Time = average_Search_Trial_Reaction_Time;
		result.catch_Trials_Correct = catch_Trials_Correct;
		result.catch_Trials_Incorrect = catch_Trials_Incorrect;
		result.catch_Trial_Accuracy = catch_Trial_Accuracy;
		result.average_Catch_Trial_Reaction_Time = average_Catch_Trial_Reaction_Time;
		result.total_Trials_Correct = total_Trials_Correct;
		result.total_Trials_Incorrect = total_Trials_Incorrect;
		return result;
	}

	private string s(int value, bool sub){
		if (value <= 0) return ""; //if we have initial option selected
		if (sub) value -= 1;
		return (value).ToString(); //return updated value
	}

	public void Save(List<string> gameData, bool comprehension)
	{
		string dob_text = dob.text;
		int dob_year = 0;
		int dob_month = 0;
		int dob_day = 0;
		float age_num = 0f;
		System.DateTime dob_date;
		List<string> otherData = new List<string>();

		comprehension_enabled = comprehension;
	
		//Calculate age

		// Parse for year, month, and day
		if (dob_text.Length >= 10){
			int.TryParse(dob_text.Substring(dob_text.Length - 4, 4), out dob_year);
			int.TryParse(dob_text.Substring(dob_text.Length - 7, 2), out dob_day);
			int.TryParse(dob_text.Substring(dob_text.Length - 10, 2), out dob_month);
		}
		string age = "";

		try{
			dob_date = new System.DateTime(dob_year, dob_month, dob_day);
			age_num += (System.DateTime.Today.Year - dob_year);
			dob_date = dob_date.AddYears((int)age_num);
			age_num += (System.DateTime.Today.Month - dob_month)/12.0f;
			age_num += (System.DateTime.Today.Day - dob_day)/365.0f;
			age = age_num.ToString();
		}
		catch(System.ArgumentOutOfRangeException){}

		//Add demographic data to our entry
		otherData.AddRange(new string[]{ System.DateTime.Now.ToString(), s(version.value, false), dob.text, age, s(fall_or_spring.value, false), s(adhd.value, true), s(adhdpres.value, false)});

		//Remove commas from our text fields
		Q1.text = System.String.Concat(Q1.text.Select((c) => c==',' ? ' ' : c));
		Q2.text = System.String.Concat(Q2.text.Select((c) => c==',' ? ' ' : c));
		Q3.text = System.String.Concat(Q3.text.Select((c) => c==',' ? ' ' : c));
		Q4.text = System.String.Concat(Q4.text.Select((c) => c==',' ? ' ' : c));
		Q5.text = System.String.Concat(Q5.text.Select((c) => c==',' ? ' ' : c));
		Q6.text = System.String.Concat(Q6.text.Select((c) => c==',' ? ' ' : c));


		//Add comprehension data (when applicable)
		if (comprehension_enabled) otherData.AddRange(new string[] {Q1.text, Q2.text, Q3.text, Q4.text, Q5.text, Q6.text});
	
		string[] info = new string[gameData.Count + otherData.Count];
		otherData.CopyTo(info, 0);
		gameData.CopyTo(info, otherData.Count);
		SaveData(ID.text, info);
	}

	static string DataFilePath;
	static string BackupAttentionDataPath = @"ResearchData/BackupAttentionData/";

	// Data Header -- NO IN-STRING COMMAS ALLOWED (it's in a .csv)
	static string[] InfoHeader = {"ID"};

	static string[] DataHeader;

	static void InitializeDataFilePath(){
		if (comprehension_enabled) DataFilePath = @"ResearchData/AttentionData.csv";
		else DataFilePath = @"ResearchData/NoComprehensionAttentionData.csv";
	}

	static void InitializeDataHeader(){
		//Question to ponder: is there a less costly way of doing this without 
		//having to create and re-append the string?
		List<string> info = new List<string>();
		info.AddRange(new string[] {"Trial Date", "Version", "Date of Birth", "Age", "Fall or Spring", "ADHD", "ADHD Presentation"});
		if (comprehension_enabled) info.AddRange(new string[] {"Q1 comments", "Q2 comments", "Q3 comments", "Q4 comments", "Q5 comments", "Q6 comment", "Q1 score", "Q2 score", "Q3 score", "Q4 score","Q5 score", "Q6 score", "Total Correct Q1Q6", "Percent Correct Q1Q6", "Book Enjoyment", "Recommend Book to Friend", "Reading Enjoyment"});
		info.AddRange(new string[] {"Total Time", "Search Trials Correct", "Search Trials Incorrect", "Search Trial Accuracy", "Average Search Trial Reaction Time", "Catch Trials Correct", "Catch Trials Incorrect", "Catch Trial Accuracy", "Average Catch Trial Reaction Time", "Total Trials Correct", "Total Trials Incorrect"});
		DataHeader = new string[info.Count];
		info.CopyTo(DataHeader, 0);
	}

	static char[] fieldSeparator = { ',' };


	// Call this to save trial data into the .csv file
	public static void SaveData(string ID, string[] newData) {
		InitializeDataFilePath();
		InitializeDataHeader();

		if (string.Equals (ID, "")) {
			ID = "(No ID entered)";
		}

		string[] newInfoArray = createInfoArray (ID);

		// Save backup of data
		SaveBackup(ID, newInfoArray, newData);

		// Read the current data and make sure the header is correct
		string[][] AllData = readCSVFile (DataFilePath);

		// Get ID
		int lineIndex = findID (AllData, ID);

		// If ID not found in the previous data, save in a new line
		if (lineIndex < 0) {
			string[][] newAllData = new string[AllData.Length + 1][];
			AllData.CopyTo (newAllData, 0);

			string[] newLine = new string[newInfoArray.Length]; // Does this give the right length?
			lineIndex = newAllData.Length - 1;
			newAllData [lineIndex] = newLine;
			newInfoArray.CopyTo (newAllData [lineIndex], 0);

			AllData = newAllData;
		}
			
		// Make sure the newData is the right length
		if (newData.Length != DataHeader.Length) {
			Debug.LogError (@"Unexpected data array length for attention data 
				saving");
		}

		// Copy into the data array
		string[] prevData = AllData[lineIndex];
		AllData[lineIndex] = new string[prevData.Length + newData.Length];
		prevData.CopyTo (AllData [lineIndex], 0);
		newData.CopyTo (AllData [lineIndex], prevData.Length);

		// Create the data header and copy over the top of the file
		int lineLength = maxDataRowLength(AllData);
		makeDataHeader(lineLength).CopyTo (AllData, 0);

		// Convert the data to one string and write it to the file
		string dataString = convertToString (AllData);
		File.WriteAllText (DataFilePath, dataString);
		Debug.Log ("Attention data successfully saved");
		Application.Quit();
	}


	// Read the data from a .csv and return it as a string[][]
	private static string[][] readCSVFile (string path) {
		string[][] allData;
		if (File.Exists (path)) {
			string[] lines = File.ReadAllLines (path);
			if (lines.Length > 1) {
				allData = new string[lines.Length][];

				for (int i=0; i < lines.Length; i++) {
					string[] fields = lines [i].Split (fieldSeparator);
					allData [i] = new string[fields.Length];
					fields.CopyTo (allData [i], 0);
				}
				return allData;
			}
		}

		// File has not been initialized
		allData = new string[1][];
		return allData;
	}


	// Search for a string[] with the same ID at index 0
	private static int findID(string[][] data, string ID) {
		for (int i = 1; i < data.Length; i++) {
			if (data [i] [0] == ID) {
				return i;
			}
		}

		return -1;
	}

	// Create the info array
	private static string[] createInfoArray (string ID) {
		string[] infoArray = new string[InfoHeader.Length];

		infoArray [0] = ID;
		//infoArray [1] = universalGM._group.ToString();

		return infoArray;
	}


	// Set up the header for the data CSV
	private static string[][] makeDataHeader(int lineLength) {
		string[][] header = new string[1][];
		//header [0] = new string[lineLength + DataHeader.Length];
		int numberOfHeaders = Mathf.Min (((lineLength - 1) / DataHeader.Length) + 1, 6);
		header [0] = new string[InfoHeader.Length + DataHeader.Length * numberOfHeaders];

		// Header line 1 (Column titles)
		InfoHeader.CopyTo(header[0], 0);

		for (int i = 0; i < numberOfHeaders; i++) {
			string prefix = "Day " + (i+1).ToString() + "_";
			string[] prefixedHeader = addPrefix(DataHeader, prefix);
			prefixedHeader.CopyTo (header[0], InfoHeader.Length + i * DataHeader.Length);
		}

		return header;
	}

	// Add a prefix to every string in a string[]
	private static string[] addPrefix (string[] array, string prefix) {
		string[] newArray = new string[array.Length];
		for (int i = 0; i < array.Length; i++) {
			newArray[i] = prefix + array[i];
		}
		return newArray;
	}


	// Convert string[][] to string
	private static string convertToString(string[][] TwoDArray) {
		string output = "";
		foreach (string[] line in TwoDArray) {
			output = output + string.Join (",", line) + "\n";
		}
		return output;
	}


	// Get the max data row length
	private static int maxDataRowLength (string[][] currentData) {
		int maxLength = 1;

		for (int i = 1; i < currentData.Length; i++) {
			for (int j = 1; j < currentData [i].Length; j++) {
				if (currentData[i][j] != "" && j > maxLength) {
					maxLength = j;
				}
			}
		}

		return maxLength;
	}


	// Save the new line of data to the backup folder for the kiddo
	private static void SaveBackup (string ID, string[] infoLine, string[] newData) {
		string infoString = string.Join (",", infoLine);
		string dataString = string.Join (",", newData);

		dataString = string.Concat(infoString, ",", dataString, "\n");

		string path = BackupAttentionDataPath + ID + ".csv";
		if (!File.Exists (path)) {
			Directory.CreateDirectory (BackupAttentionDataPath);
		}
		File.AppendAllText (path, dataString);
		Debug.Log ("Backup attention data saved");
	}
}
