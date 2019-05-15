﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.persistence.entity
{

	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;
	using SchemaLogQuery = org.camunda.bpm.engine.management.SchemaLogQuery;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogManager : AbstractManager
	{

	  public virtual long? findSchemaLogEntryCountByQueryCriteria(SchemaLogQuery schemaLogQuery)
	  {
		if (Authorized)
		{
		  return (long?) DbEntityManager.selectOne("selectSchemaLogEntryCountByQueryCriteria", schemaLogQuery);
		}
		else
		{
		  return 0L;
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.camunda.bpm.engine.management.SchemaLogEntry> findSchemaLogEntriesByQueryCriteria(org.camunda.bpm.engine.impl.SchemaLogQueryImpl schemaLogQueryImpl, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<SchemaLogEntry> findSchemaLogEntriesByQueryCriteria(SchemaLogQueryImpl schemaLogQueryImpl, Page page)
	  {
		if (Authorized)
		{
		  return DbEntityManager.selectList("selectSchemaLogEntryByQueryCriteria", schemaLogQueryImpl, page);
		}
		else
		{
		  return Collections.emptyList();
		}
	  }

	  private bool Authorized
	  {
		  get
		  {
			try
			{
			  AuthorizationManager.checkCamundaAdmin();
			  return true;
			}
			catch (AuthorizationException)
			{
			  return false;
			}
		  }
	  }
	}

}