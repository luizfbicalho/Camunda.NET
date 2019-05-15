﻿/*
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

	/// <summary>
	/// holds the parameters of a page (partial result) for a query. 
	/// 
	/// @author Joram Barrez
	/// @author Tom Baeyens
	/// </summary>
	public class Page
	{

	  protected internal int firstResult;
	  protected internal int maxResults;

	  public Page(int firstResult, int maxResults)
	  {
		this.firstResult = firstResult;
		this.maxResults = maxResults;
	  }

	  public virtual int FirstResult
	  {
		  get
		  {
			return firstResult;
		  }
	  }

	  public virtual int MaxResults
	  {
		  get
		  {
			return maxResults;
		  }
	  }
	}

}