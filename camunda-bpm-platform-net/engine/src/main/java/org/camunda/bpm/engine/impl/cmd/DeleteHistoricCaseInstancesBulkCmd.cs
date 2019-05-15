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
namespace org.camunda.bpm.engine.impl.cmd
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotEmpty;


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;

	[Serializable]
	public class DeleteHistoricCaseInstancesBulkCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;

	  protected internal readonly IList<string> caseInstanceIds;

	  public DeleteHistoricCaseInstancesBulkCmd(IList<string> caseInstanceIds)
	  {
		this.caseInstanceIds = caseInstanceIds;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotEmpty(typeof(BadUserRequestException), "caseInstanceIds", caseInstanceIds);

		// Check if case instances are all closed
		commandContext.runWithoutAuthorization(new CallableAnonymousInnerClass(this));

		commandContext.OperationLogManager.logCaseInstanceOperation(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY, null, Collections.singletonList(new PropertyChange("nrOfInstances", null, caseInstanceIds.Count)));

		commandContext.HistoricCaseInstanceManager.deleteHistoricCaseInstancesByIds(caseInstanceIds);

		return null;
	  }

	  private class CallableAnonymousInnerClass : Callable<Void>
	  {
		  private readonly DeleteHistoricCaseInstancesBulkCmd outerInstance;

		  public CallableAnonymousInnerClass(DeleteHistoricCaseInstancesBulkCmd outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			ensureEquals(typeof(BadUserRequestException), "ClosedCaseInstanceIds", (new HistoricCaseInstanceQueryImpl()).closed().caseInstanceIds(new HashSet<string>(outerInstance.caseInstanceIds)).count(), outerInstance.caseInstanceIds.Count);
			return null;
		  }
	  }

	}
}