using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestSpecialChar : MonoBehaviour {
	void Start(){
		RunTests ();
	}
	
	void RunTests(){
		TestEscapeSpecials();
	}
	
	static Dictionary<string, string> testCases = new Dictionary<string, string>(){
		{"1", "hi\""},
		{"test_double_quotes", "she said, \"What?\""},
		{"test_backslash_escape", "inner vocab \\"},
		{"double_backslash_escape", "\\\\"},
		{"blackslash_double_quote", "\\\""},
		{"test_double_quote","hi\""},
		{"test_double_backslash_escape","\\"},
		{"test_blackslash_double_quote","\\\""},
		{"test_backslash_escape2","inner vocab \""},
	};

	static Dictionary<string, string> testCasesJSONRepresentation = new Dictionary<string, string>(){
		{"1", "{\"1\":\"hi\\\"\"}"},
		{"test_double_quotes", "{\"test_double_quotes\":\"she said, \\\"What?\\\"\"}"},
		{"test_backslash_escape", "{\"test_backslash_escape\":\"inner vocab \\\\\"}"},
		
		{"double_backslash_escape", "{\"double_backslash_escape\":\"\\\\\\\\\"}"},
		{"blackslash_double_quote", "{\"blackslash_double_quote\":\"\\\\\\\"\"}"},
		{"test_double_quote","{\"test_double_quote\":\"hi\\\"\"}"},
		{"test_double_backslash_escape","{\"test_double_backslash_escape\":\"\\\\\"}"},
		{"test_blackslash_double_quote","{\"test_blackslash_double_quote\":\"\\\\\\\"\"}"},
		{"test_backslash_escape2","{\"test_backslash_escape2\":\"inner vocab \\\"\"}"},
	};
	
	//replaces " and \ with escaped versions
	string SimpleJSONConstructor(string key, string value){
		return "{" + "\"" + key + "\"" + ":" + "\"" + value.Replace ("\\", "\\\\").Replace("\"", "\\\"") + "\"" +"}";
	}
	
	bool TestEqual(bool b1, bool b2){
		return b1 == b2;
	}
	
	bool AssertStrEqual(string s1, string s2, string testCaseName = null){
		bool res = TestEqual (true, s1 == s2);
		if (res)
			Debug.Log ("PASSED [" + testCaseName + "]: " + s1 + " vs " + s2);
		else Debug.LogError("FAILED [" + testCaseName + "] " + s1 + " vs " + s2);
		return res;
	}
	
	void TestEscapeSpecials(){
		foreach (KeyValuePair<string, string> keys in testCases) {
			string key = keys.Key;
			Debug.Log ("TEST [Form JSON]: " + key);
			
			JSONObject obj = new JSONObject();
			obj.AddField(key, testCases[key]);
			AssertStrEqual(testCases[key], obj[key].str, "storage of value");
			
			string simpleJSON = SimpleJSONConstructor(key, testCases[key]);
			AssertStrEqual(simpleJSON, testCasesJSONRepresentation[key], "simpleJSON match correct JSON");
			
			string stringRepresentation = obj.ToString();
			string printRepresentation = obj.Print();
			bool createJSONCorrectly = AssertStrEqual(simpleJSON, stringRepresentation, "match ToString() to correct JSON");
			AssertStrEqual(simpleJSON, printRepresentation, "match Print() to correct JSON");
			
			Debug.Log ("TEST [Reform JSON]: " + key);
			try {
				Debug.Log("string representation: " + stringRepresentation);
				
				JSONObject resultObj = new JSONObject(stringRepresentation);
				
				Debug.Log("from constructor: " + resultObj.ToString());
				
				AssertStrEqual(resultObj.ToString(), simpleJSON, "result from JSONObject constructor using ToString() result");
				AssertStrEqual(resultObj[key].str, testCases[key]);
			} catch (Exception e){
				Debug.LogError("Problem reforming json. " + e.ToString());
			}
			
			Debug.Log ("TEST [Reform JSON using result from SimpleJSONConstructor]: " + key);
			try {
				JSONObject resultObj = new JSONObject(simpleJSON);
				AssertStrEqual(resultObj.ToString(), simpleJSON, "result from JSONObject constructor using correct JSON");
				AssertStrEqual(resultObj[key].str, testCases[key]);
			} catch (Exception e){
				Debug.LogError("Problem reforming json using correct JSON: " + simpleJSON + " " + e.ToString());
			}
			Debug.Log ("******");
			
		}
	}
}

