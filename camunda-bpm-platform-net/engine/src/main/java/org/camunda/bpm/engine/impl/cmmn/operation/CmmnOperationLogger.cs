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
namespace org.camunda.bpm.engine.impl.cmmn.operation
{
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class CmmnOperationLogger : ProcessEngineLogger
	{

	  public virtual void completingSubCaseError(CmmnExecution execution, Exception cause)
	  {
		logError("001", "Error while completing sub case of case execution '{}'. Reason: '{}'", execution, cause.Message, cause);
	  }

	  public virtual ProcessEngineException completingSubCaseErrorException(CmmnExecution execution, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("002", "Error while completing sub case of case execution '{}'.", execution), cause);
	  }

	  public virtual BadUserRequestException exceptionCreateCaseInstanceByIdAndTenantId()
	  {
		return new BadUserRequestException(exceptionMessage("003", "Cannot specify a tenant-id when create a case instance by case definition id."));
	  }

	}

}