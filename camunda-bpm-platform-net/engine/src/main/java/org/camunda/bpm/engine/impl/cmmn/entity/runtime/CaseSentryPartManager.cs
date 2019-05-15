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
namespace org.camunda.bpm.engine.impl.cmmn.entity.runtime
{

	using AbstractManager = org.camunda.bpm.engine.impl.persistence.AbstractManager;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseSentryPartManager : AbstractManager
	{

	  public virtual void insertCaseSentryPart(CaseSentryPartEntity caseSentryPart)
	  {
		DbEntityManager.insert(caseSentryPart);
	  }

	  public virtual void deleteSentryPart(CaseSentryPartEntity caseSentryPart)
	  {
		DbEntityManager.delete(caseSentryPart);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<CaseSentryPartEntity> findCaseSentryPartsByCaseExecutionId(String caseExecutionId)
	  public virtual IList<CaseSentryPartEntity> findCaseSentryPartsByCaseExecutionId(string caseExecutionId)
	  {
		return DbEntityManager.selectList("selectCaseSentryPartsByCaseExecutionId", caseExecutionId);
	  }

	  public virtual long findCaseSentryPartCountByQueryCriteria(CaseSentryPartQueryImpl caseSentryPartQuery)
	  {
		return (long?) DbEntityManager.selectOne("selectCaseSentryPartsCountByQueryCriteria", caseSentryPartQuery).Value;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<CaseSentryPartEntity> findCaseSentryPartByQueryCriteria(CaseSentryPartQueryImpl caseSentryPartQuery, org.camunda.bpm.engine.impl.Page page)
	  public virtual IList<CaseSentryPartEntity> findCaseSentryPartByQueryCriteria(CaseSentryPartQueryImpl caseSentryPartQuery, Page page)
	  {
		return DbEntityManager.selectList("selectCaseSentryPartsByQueryCriteria", caseSentryPartQuery, page);
	  }

	}

}