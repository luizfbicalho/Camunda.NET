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
namespace org.camunda.bpm.qa.performance.engine.util
{

	using TabularResultSet = org.camunda.bpm.qa.performance.engine.framework.aggregate.TabularResultSet;

	/// <summary>
	/// <para>Provides export functionality for exporting a <seealso cref="TabularResultSet"/>
	/// to CSV (Comma Separated Values).</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CsvUtil
	{

	  public static string resultSetAsCsv(TabularResultSet resultSet)
	  {
		return resultSetAsCsv(resultSet, true);
	  }

	  public static string resultSetAsCsvLine(TabularResultSet resultSet)
	  {
		return resultSetAsCsv(resultSet, false);
	  }

	  private static string resultSetAsCsv(TabularResultSet resultSet, bool writeHeadline)
	  {
		StringBuilder builder = new StringBuilder();

		if (writeHeadline)
		{
		  // write headline
		  IList<string> resultColumnNames = resultSet.ResultColumnNames;
		  for (int i = 0; i < resultColumnNames.Count; i++)
		  {
			builder.Append(resultColumnNames[i]);
			builder.Append(";");
		  }
		  builder.Append("\n");
		}

		// write results
		IList<IList<object>> results = resultSet.Results;
		foreach (IList<object> row in results)
		{
		  foreach (object @object in row)
		  {
			builder.Append(@object.ToString());
			builder.Append(";");
		  }
		  builder.Append("\n");
		}

		return builder.ToString();
	  }

	  public static void saveResultSetToFile(string fileName, TabularResultSet resultSet)
	  {
		FileUtil.writeStringToFile(resultSetAsCsv(resultSet), fileName);
	  }

	}

}