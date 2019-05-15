using System;
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
namespace org.camunda.bpm.engine.impl.db
{


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class ListQueryParameterObject
	{

	  private const long serialVersionUID = 1L;

	  protected internal AuthorizationCheck authCheck = new AuthorizationCheck();

	  protected internal TenantCheck tenantCheck = new TenantCheck();
	  protected internal IList<QueryOrderingProperty> orderingProperties = new List<QueryOrderingProperty>();

	  protected internal int maxResults = int.MaxValue;
	  protected internal int firstResult = 0;
	  protected internal object parameter;
	  protected internal string databaseType;

	  public ListQueryParameterObject()
	  {
	  }

	  public ListQueryParameterObject(object parameter, int firstResult, int maxResults)
	  {
		this.parameter = parameter;
		this.firstResult = firstResult;
		this.maxResults = maxResults;
	  }

	  public virtual int FirstResult
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

	  public virtual int FirstRow
	  {
		  get
		  {
			return firstResult + 1;
		  }
	  }

	  public virtual int LastRow
	  {
		  get
		  {
			if (maxResults == int.MaxValue)
			{
			  return maxResults;
			}
			return firstResult + maxResults + 1;
		  }
	  }

	  public virtual int MaxResults
	  {
		  get
		  {
			return maxResults;
		  }
		  set
		  {
			this.maxResults = value;
		  }
	  }

	  public virtual object Parameter
	  {
		  get
		  {
			return parameter;
		  }
		  set
		  {
			this.parameter = value;
		  }
	  }




	  public virtual string DatabaseType
	  {
		  set
		  {
			this.databaseType = value;
		  }
		  get
		  {
			return databaseType;
		  }
	  }


	  public virtual AuthorizationCheck AuthCheck
	  {
		  get
		  {
			return authCheck;
		  }
		  set
		  {
			this.authCheck = value;
		  }
	  }


	  public virtual TenantCheck TenantCheck
	  {
		  get
		  {
			return tenantCheck;
		  }
		  set
		  {
			this.tenantCheck = value;
		  }
	  }


	  public virtual IList<QueryOrderingProperty> OrderingProperties
	  {
		  get
		  {
			return orderingProperties;
		  }
		  set
		  {
			this.orderingProperties = value;
		  }
	  }

	}

}