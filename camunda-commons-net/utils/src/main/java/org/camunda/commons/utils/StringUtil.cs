using System;
using System.Text;

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
namespace org.camunda.commons.utils
{

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public sealed class StringUtil
	{

	  /// <summary>
	  /// Checks whether a <seealso cref="string"/> seams to be an expression or not
	  /// </summary>
	  /// <param name="text"> the text to check </param>
	  /// <returns> true if the text seams to be an expression false otherwise </returns>
	  public static bool isExpression(string text)
	  {
		if (string.ReferenceEquals(text, null))
		{
		  return false;
		}
		text = text.Trim();
		return text.StartsWith("${", StringComparison.Ordinal) || text.StartsWith("#{", StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Splits a <seealso cref="string"/> by an expression.
	  /// </summary>
	  /// <param name="text"> the text to split </param>
	  /// <param name="regex"> the regex to split by </param>
	  /// <returns> the parts of the text or null if text was null </returns>
	  public static string[] split(string text, string regex)
	  {
		if (string.ReferenceEquals(text, null))
		{
		  return null;
		}
		else if (string.ReferenceEquals(regex, null))
		{
		  return new string[] {text};
		}
		else
		{
		  string[] result = text.Split(regex, true);
		  for (int i = 0; i < result.Length; i++)
		  {
			result[i] = result[i].Trim();
		  }
		  return result;
		}
	  }

	  /// <summary>
	  /// Joins a list of Strings to a single one.
	  /// </summary>
	  /// <param name="delimiter"> the delimiter between the joined parts </param>
	  /// <param name="parts"> the parts to join </param>
	  /// <returns> the joined String or null if parts was null </returns>
	  public static string join(string delimiter, params string[] parts)
	  {
		if (parts == null)
		{
		  return null;
		}

		if (string.ReferenceEquals(delimiter, null))
		{
		  delimiter = "";
		}

		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < parts.Length; i++)
		{
		  if (i > 0)
		  {
			stringBuilder.Append(delimiter);
		  }
		  stringBuilder.Append(parts[i]);
		}
		return stringBuilder.ToString();
	  }

	  /// <summary>
	  /// Returns either the passed in String, or if the String is <code>null</code>, an empty String ("").
	  /// 
	  /// <pre>
	  /// StringUtils.defaultString(null)  = ""
	  /// StringUtils.defaultString("")    = ""
	  /// StringUtils.defaultString("bat") = "bat"
	  /// </pre>
	  /// </summary>
	  /// <param name="text">  the String to check, may be null </param>
	  /// <returns> the passed in String, or the empty String if it  was <code>null</code> </returns>
	  public static string defaultString(string text)
	  {
		  return string.ReferenceEquals(text, null) ? "" : text;
	  }

	  /// <summary>
	  /// Fetches the stack trace of an exception as a String.
	  /// </summary>
	  /// <param name="throwable"> to get the stack trace from </param>
	  /// <returns> the stack trace as String </returns>
	  public static string getStackTrace(Exception throwable)
	  {
		StringWriter sw = new StringWriter();
		throwable.printStackTrace(new PrintWriter(sw, true));
		return sw.ToString();
	  }

	}

}