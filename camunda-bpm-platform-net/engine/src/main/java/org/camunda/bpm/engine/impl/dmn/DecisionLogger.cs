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
namespace org.camunda.bpm.engine.impl.dmn
{

	using DmnDecisionResult = org.camunda.bpm.dmn.engine.DmnDecisionResult;
	using DmnEngineException = org.camunda.bpm.dmn.engine.DmnEngineException;
	using DecisionResultMapper = org.camunda.bpm.engine.impl.dmn.result.DecisionResultMapper;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DecisionLogger : ProcessEngineLogger
	{

	  public virtual ProcessEngineException decisionResultMappingException(DmnDecisionResult decisionResult, DecisionResultMapper resultMapper, DmnEngineException cause)
	  {
		return new ProcessEngineException(exceptionMessage("001", "The decision result mapper '{}' failed to process '{}'", resultMapper, decisionResult), cause);
	  }

	  public virtual ProcessEngineException decisionResultCollectMappingException(ICollection<string> outputNames, DmnDecisionResult decisionResult, DecisionResultMapper resultMapper)
	  {
		return new ProcessEngineException(exceptionMessage("002", "The decision result mapper '{}' failed to process '{}'. The decision outputs should only contains values for one output name but found '{}'.", resultMapper, decisionResult, outputNames));
	  }

	  public virtual BadUserRequestException exceptionEvaluateDecisionDefinitionByIdAndTenantId()
	  {
		return new BadUserRequestException(exceptionMessage("003", "Cannot specify a tenant-id when evaluate a decision definition by decision definition id."));
	  }

	  public virtual ProcessEngineException exceptionParseDmnResource(string resouceName, Exception cause)
	  {
		return new ProcessEngineException(exceptionMessage("004", "Unable to transform DMN resource '{}'.", resouceName), cause);
	  }

	  public virtual ProcessEngineException exceptionNoDrdForResource(string resourceName)
	  {
		return new ProcessEngineException(exceptionMessage("005", "Found no decision requirements definition for DMN resource '{}'.", resourceName));
	  }

	}

}