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
namespace org.camunda.bpm.engine.impl.cmmn.model
{
	using CaseExecutionImpl = org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionImpl;
	using CmmnCaseInstance = org.camunda.bpm.engine.impl.cmmn.execution.CmmnCaseInstance;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;


	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[Serializable]
	public class CmmnCaseDefinition : CmmnActivity
	{

	  private const long serialVersionUID = 1L;

	  public CmmnCaseDefinition(string id) : base(id, null)
	  {
		caseDefinition = this;
	  }

	  public virtual CmmnCaseInstance createCaseInstance()
	  {
		return createCaseInstance(null);
	  }

	  public virtual CmmnCaseInstance createCaseInstance(string businessKey)
	  {

		// create a new case instance
		CmmnExecution caseInstance = newCaseInstance();

		// set the definition...
		caseInstance.CaseDefinition = this;
		// ... and the case instance (identity)
		caseInstance.CaseInstance = caseInstance;

		// set the business key
		caseInstance.BusinessKey = businessKey;

		// get the case plan model as "initial" activity
		CmmnActivity casePlanModel = Activities[0];

		// set the case plan model activity
		caseInstance.Activity = casePlanModel;

		return caseInstance;
	  }

	  protected internal virtual CmmnExecution newCaseInstance()
	  {
		return new CaseExecutionImpl();
	  }

	}

}