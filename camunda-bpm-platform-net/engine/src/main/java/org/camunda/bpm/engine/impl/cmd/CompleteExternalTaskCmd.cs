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
namespace org.camunda.bpm.engine.impl.cmd
{

	using ExternalTaskEntity = org.camunda.bpm.engine.impl.persistence.entity.ExternalTaskEntity;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public class CompleteExternalTaskCmd : HandleExternalTaskCmd
	{

	  protected internal IDictionary<string, object> variables;
	  protected internal IDictionary<string, object> localVariables;

	  public CompleteExternalTaskCmd(string externalTaskId, string workerId, IDictionary<string, object> variables, IDictionary<string, object> localVariables) : base(externalTaskId, workerId)
	  {
		this.localVariables = localVariables;
		this.variables = variables;
	  }

	  public override string ErrorMessageOnWrongWorkerAccess
	  {
		  get
		  {
			return "External Task " + externalTaskId + " cannot be completed by worker '" + workerId;
		  }
	  }

	  public virtual void execute(ExternalTaskEntity externalTask)
	  {
		externalTask.complete(variables, localVariables);
	  }
	}

}