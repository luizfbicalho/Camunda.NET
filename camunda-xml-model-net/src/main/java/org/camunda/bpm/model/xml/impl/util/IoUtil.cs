using System;
using System.Text;
using System.IO;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.model.xml.impl.util
{
	using DomDocument = org.camunda.bpm.model.xml.instance.DomDocument;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// 
	/// </summary>
	public sealed class IoUtil
	{

	  public static void closeSilently(System.IDisposable closeable)
	  {
		try
		{
		  if (closeable != null)
		  {
			closeable.Dispose();
		  }
		}
		catch (Exception)
		{
		  // ignored
		}
	  }

	  /// <summary>
	  /// Convert an <seealso cref="System.IO.Stream_Input"/> to a <seealso cref="string"/>
	  /// </summary>
	  /// <param name="inputStream"> the <seealso cref="System.IO.Stream_Input"/> to convert </param>
	  /// <returns> the resulting <seealso cref="string"/> </returns>
	  /// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String getStringFromInputStream(InputStream inputStream) throws IOException
	  public static string getStringFromInputStream(Stream inputStream)
	  {
		return getStringFromInputStream(inputStream, true);
	  }

	  /// <summary>
	  /// Convert an <seealso cref="System.IO.Stream_Input"/> to a <seealso cref="string"/>
	  /// </summary>
	  /// <param name="inputStream"> the <seealso cref="System.IO.Stream_Input"/> to convert </param>
	  /// <param name="trim"> trigger if whitespaces are trimmed in the output </param>
	  /// <returns> the resulting <seealso cref="string"/> </returns>
	  /// <exception cref="IOException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static String getStringFromInputStream(InputStream inputStream, boolean trim) throws IOException
	  private static string getStringFromInputStream(Stream inputStream, bool trim)
	  {
		StreamReader bufferedReader = null;
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
		  bufferedReader = new StreamReader(inputStream);
		  string line;
		  while (!string.ReferenceEquals((line = bufferedReader.ReadLine()), null))
		  {
			if (trim)
			{
			  stringBuilder.Append(line.Trim());
			}
			else
			{
			  stringBuilder.Append(line).Append("\n");
			}
		  }
		}
		finally
		{
		  closeSilently(bufferedReader);
		}

		return stringBuilder.ToString();
	  }

	  /// <summary>
	  /// Converts a <seealso cref="System.IO.Stream_Output"/> to an <seealso cref="System.IO.Stream_Input"/> by coping the data directly.
	  /// WARNING: Do not use for large data (>100MB). Only for testing purpose.
	  /// </summary>
	  /// <param name="outputStream"> the <seealso cref="System.IO.Stream_Output"/> to convert </param>
	  /// <returns> the resulting <seealso cref="System.IO.Stream_Input"/> </returns>
	  public static Stream convertOutputStreamToInputStream(Stream outputStream)
	  {
		sbyte[] data = ((MemoryStream) outputStream).toByteArray();
		return new MemoryStream(data);
	  }

	  /// <summary>
	  /// Converts a <seealso cref="DomDocument"/> to its String representation
	  /// </summary>
	  /// <param name="document">  the XML document to convert </param>
	  public static string convertXmlDocumentToString(DomDocument document)
	  {
		StringWriter stringWriter = new StringWriter();
		StreamResult result = new StreamResult(stringWriter);
		transformDocumentToXml(document, result);
		return stringWriter.ToString();
	  }

	  /// <summary>
	  /// Writes a <seealso cref="DomDocument"/> to an <seealso cref="System.IO.Stream_Output"/> by transforming the DOM to XML.
	  /// </summary>
	  /// <param name="document">  the DOM document to write </param>
	  /// <param name="outputStream">  the <seealso cref="System.IO.Stream_Output"/> to write to </param>
	  public static void writeDocumentToOutputStream(DomDocument document, Stream outputStream)
	  {
		StreamResult result = new StreamResult(outputStream);
		transformDocumentToXml(document, result);
	  }

	  /// <summary>
	  /// Transforms a <seealso cref="DomDocument"/> to XML output.
	  /// </summary>
	  /// <param name="document">  the DOM document to transform </param>
	  /// <param name="result">  the <seealso cref="StreamResult"/> to write to </param>
	  public static void transformDocumentToXml(DomDocument document, StreamResult result)
	  {
		TransformerFactory transformerFactory = TransformerFactory.newInstance();
		try
		{
		  Transformer transformer = transformerFactory.newTransformer();
		  transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
		  transformer.setOutputProperty(OutputKeys.INDENT, "yes");
		  transformer.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "2");

		  lock (document)
		  {
			transformer.transform(document.DomSource, result);
		  }
		}
		catch (TransformerConfigurationException e)
		{
		  throw new ModelIoException("Unable to create a transformer for the model", e);
		}
		catch (TransformerException e)
		{
		  throw new ModelIoException("Unable to transform model to xml", e);
		}
	  }

	}

}