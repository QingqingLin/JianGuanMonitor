using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Xml;

namespace MonitorPorts
{
	/// <summary>
	/// Summary description for Protocols.
	/// </summary>
	/// 

	public class Protocols
	{
		static Protocols()
		{
			readFileNames();
			for (int i=0;i<Keys.Count;i++)
			{
				addProtocoltoSys(Keys[i].ToString());
			}
		}

		public static Hashtable protoDefs = new Hashtable();
		public static ArrayList Keys = new ArrayList();//id leri tutuyor.

		public static void readFileNames()
		{
			string str = xmlReader("Protocols");
			XmlDocument doc1 = new XmlDocument();
			doc1.LoadXml(str);
			XmlNode root = doc1.DocumentElement;
			//			root=root.FirstChild;			
			if (root.Name == "protocol")
			{
				foreach (XmlNode item in root.ChildNodes)
				{
					Keys.Add(item.InnerText.ToString());
				}
			}
		}

		public static string getProtocolName(string protoId){
			return (string)((Hashtable)(protoDefs[protoId]))["name"];
		}

		public static void removeProtocol(string protoId)
		{
			string str = xmlReader("Protocols");
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(str);
			XmlNode root = doc.DocumentElement;
			//			root=root.FirstChild;			
			if (root.Name == "protocol")
			{
				foreach (XmlNode item in root.ChildNodes)
				{
					if (item.InnerText.ToString()==protoId)
						root.RemoveChild(item);
				}
			}

			XmlTextWriter writer = new XmlTextWriter("Protocols.xml",null);
			writer.Formatting = Formatting.Indented;
			doc.Save(writer);			
			if (writer!=null)
				writer.Close();

			if(protoDefs.Contains(protoId))
				protoDefs.Remove(protoId);
			if(Keys.Contains(protoId))
				Keys.Remove(protoId);

		}

		public static void addProtocoltoSys(string protoid)
		{
			Hashtable xxx = new Hashtable();
			string str = xmlReader(protoid);
			if (str!="")
			{
				XmlDocument doc1 = new XmlDocument();
				doc1.LoadXml(str);
				XmlNode root = doc1.DocumentElement;
				//			root=root.FirstChild;
				int [] pair;
				if (root.Name == "protocol")
				{
					foreach (XmlNode item in root.ChildNodes)
					{
						if (item.Name == "fields")
						{
							foreach (XmlNode it in item.ChildNodes)
							{
								pair = new int[2];
								pair[0]=Convert.ToInt32(it.ChildNodes[1].InnerText);
								pair[1]=Convert.ToInt32(it.ChildNodes[2].InnerText);
								xxx.Add(it.FirstChild.InnerText,pair);
							}
						}
						else if (item.Name == "name")
						{
							xxx.Add(item.Name.ToString(),item.InnerText);
						}
						else if (item.Name == "id")
						{
							xxx.Add(item.Name.ToString(),item.InnerText);
						}
					}
				}
				if(!protoDefs.Contains(protoid))
				{
					protoDefs.Add(protoid,xxx);				
				}
				if(!Keys.Contains(protoid))
				{
					Keys.Add(protoid);
				}
			}
		}

		public static string xmlReader(string filename)
		{
			string ret ="";
			XmlTextReader reader = null;

			try 
			{
				// Load the reader with the data file and ignore all white space nodes.         
				reader = new XmlTextReader(filename+".xml");
				reader.WhitespaceHandling = WhitespaceHandling.None;

				// Parse the file and display each of the nodes.
				while (reader.Read()) 
				{
					switch (reader.NodeType) 
					{
						case XmlNodeType.Element:
							ret+=String.Format("<{0}>", reader.Name);
							break;
						case XmlNodeType.Text:
							ret+=String.Format(reader.Value);
							break;
						case XmlNodeType.CDATA:
							ret+=String.Format("<![CDATA[{0}]]>", reader.Value);
							break;
						case XmlNodeType.ProcessingInstruction:
							ret+=String.Format("<?{0} {1}?>", reader.Name, reader.Value);
							break;
						case XmlNodeType.Comment:
							ret+=String.Format("<!--{0}-->", reader.Value);
							break;
						case XmlNodeType.XmlDeclaration:
							ret+=String.Format("<?xml version='1.0'?>");
							break;
						case XmlNodeType.Document:
							break;
						case XmlNodeType.DocumentType:
							ret+=String.Format("<!DOCTYPE {0} [{1}]", reader.Name, reader.Value);
							break;
						case XmlNodeType.EntityReference:
							ret+=String.Format(reader.Name);
							break;
						case XmlNodeType.EndElement:
							ret+=String.Format("</{0}>", reader.Name);
							break;
					}       
				}           
			}
			catch(Exception){
				if (filename=="Protocols")
				{
					if (reader!=null)
						reader.Close();
					XmlDocument doc = new XmlDocument();
					doc.LoadXml("<?xml version=\"1.0\"?><protocol></protocol>");
					XmlTextWriter writer = new XmlTextWriter("Protocols.xml",null);
					writer.Formatting = Formatting.Indented;
					doc.Save(writer);			
					if (writer!=null)
						writer.Close();
					protoDefs.Clear();
					Keys.Clear();
					ret= "<?xml version=\"1.0\"?><protocol></protocol>";
					
				}
				else{
					removeProtocol("protoId");
					ret="";
				}
			}
			if (reader!=null)
				reader.Close();
			return ret;
		}

		public static Hashtable GetProcFields(int protoId)
		{
			return (Hashtable)protoDefs[protoId.ToString()];
		}
		public static Hashtable GetProcFields(string protoId)
		{
			return (Hashtable)protoDefs[protoId];
		}

		public static bool ContainsProtocol(string protoId)
		{
			return Keys.Contains(protoId);
		}
		public static bool ContainsProtocol(int protoId)
		{
			return Keys.Contains(protoId.ToString());
		}
	}
}
