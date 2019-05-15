using System;

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
namespace org.camunda.bpm.engine.impl
{

	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TablePage = org.camunda.bpm.engine.management.TablePage;
	using TablePageQuery = org.camunda.bpm.engine.management.TablePageQuery;


	/// 
	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class TablePageQueryImpl : TablePageQuery, Command<TablePage>
	{

	  private const long serialVersionUID = 1L;

	  [NonSerialized]
	  internal CommandExecutor commandExecutor;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tableName_Renamed;
	  protected internal string order;
	  protected internal int firstResult;
	  protected internal int maxResults;

	  public TablePageQueryImpl()
	  {
	  }

	  public TablePageQueryImpl(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
	  }

	  public virtual TablePageQueryImpl tableName(string tableName)
	  {
		this.tableName_Renamed = tableName;
		return this;
	  }

	  public virtual TablePageQueryImpl orderAsc(string column)
	  {
		addOrder(column, AbstractQuery.SORTORDER_ASC);
		return this;
	  }

	  public virtual TablePageQueryImpl orderDesc(string column)
	  {
		addOrder(column, AbstractQuery.SORTORDER_DESC);
		return this;
	  }

	  public virtual string TableName
	  {
		  get
		  {
			return tableName_Renamed;
		  }
	  }

	  protected internal virtual void addOrder(string column, string sortOrder)
	  {
		if (string.ReferenceEquals(order, null))
		{
		  order = "";
		}
		else
		{
		  order = order + ", ";
		}
		order = order + column + " " + sortOrder;
	  }

	  public virtual TablePage listPage(int firstResult, int maxResults)
	  {
		this.firstResult = firstResult;
		this.maxResults = maxResults;
		return commandExecutor.execute(this);
	  }

	  public virtual TablePage execute(CommandContext commandContext)
	  {
		commandContext.AuthorizationManager.checkCamundaAdmin();
		return commandContext.TableDataManager.getTablePage(this, firstResult, maxResults);
	  }

	  public virtual string Order
	  {
		  get
		  {
			return order;
		  }
	  }

	}

}