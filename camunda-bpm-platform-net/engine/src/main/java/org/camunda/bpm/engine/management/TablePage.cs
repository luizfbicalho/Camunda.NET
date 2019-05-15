using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.management
{



	/// <summary>
	/// Data structure used for retrieving database table content.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class TablePage
	{

	  protected internal string tableName;

	  /// <summary>
	  /// The total number of rows in the table.
	  /// </summary>
	  protected internal long total = -1;

	  /// <summary>
	  /// Identifies the index of the first result stored in this TablePage. 
	  /// For example in a paginated database table, this value identifies the record number of
	  /// the result on the first row.
	  /// </summary>
	  protected internal long firstResult;

	  /// <summary>
	  /// The actual content of the database table, stored as a list of mappings of
	  /// the form {colum name, value}.
	  /// 
	  /// This means that every map object in the list corresponds with one row in
	  /// the database table.
	  /// </summary>
	  protected internal IList<IDictionary<string, object>> rowData;

	  public TablePage()
	  {

	  }

	  public virtual string TableName
	  {
		  get
		  {
			return tableName;
		  }
		  set
		  {
			this.tableName = value;
		  }
	  }


	  /// <returns> the start index of this page 
	  ///         (ie the index of the first element in the page) </returns>
	  public virtual long FirstResult
	  {
		  get
		  {
			return firstResult;
		  }
		  set
		  {
			this.firstResult = value;
		  }
	  }


	  public virtual IList<IDictionary<string, object>> Rows
	  {
		  set
		  {
			this.rowData = value;
		  }
		  get
		  {
			return rowData;
		  }
	  }


	  public virtual long Total
	  {
		  set
		  {
			this.total = value;
		  }
		  get
		  {
			return total;
		  }
	  }


	  /// <returns> the actual number of rows in this page. </returns>
	  public virtual long Size
	  {
		  get
		  {
			return rowData.Count;
		  }
	  }
	}

}