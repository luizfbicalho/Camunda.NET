using System.Collections.Generic;
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
namespace org.camunda.bpm.model.xml.impl.util
{

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public sealed class StringUtil
	{

	  private static readonly Pattern pattern = Pattern.compile("(\\w[^,]*)|([#$]\\{[^}]*})");

	  /// <summary>
	  /// Splits a comma separated list in to single Strings. The list can
	  /// contain expressions with commas in it.
	  /// </summary>
	  /// <param name="text">  the comma separated list </param>
	  /// <returns> the Strings of the list or an empty List if text is empty or null </returns>
	  public static IList<string> splitCommaSeparatedList(string text)
	  {
		if (string.ReferenceEquals(text, null) || text.Length == 0)
		{
		  return Collections.emptyList();
		}
		Matcher matcher = pattern.matcher(text);
		IList<string> parts = new List<string>();
		while (matcher.find())
		{
		  parts.Add(matcher.group().Trim());
		}
		return parts;
	  }

	  /// <summary>
	  /// Joins a list of Strings to a comma separated single String.
	  /// </summary>
	  /// <param name="list">  the list to join </param>
	  /// <returns> the resulting comma separated string or null if the list is null </returns>
	  public static string joinCommaSeparatedList(IList<string> list)
	  {
		return joinList(list, ", ");
	  }

	  public static IList<string> splitListBySeparator(string text, string separator)
	  {
		string[] result = new string[]{};
		if (!string.ReferenceEquals(text, null))
		{
		  result = text.Split(separator, true);
		}
		return new List<string>(Arrays.asList(result));
	  }

	  public static string joinList(IList<string> list, string separator)
	  {
		if (list == null)
		{
		  return null;
		}

		int size = list.Count;
		if (size == 0)
		{
		  return "";
		}
		else if (size == 1)
		{
		  return list[0];
		}
		else
		{
		  StringBuilder builder = new StringBuilder(size * 8);
		  builder.Append(list[0]);
		  foreach (object element in list.subList(1, size))
		  {
			builder.Append(separator);
			builder.Append(element);
		  }
		  return builder.ToString();
		}
	  }
	}

}