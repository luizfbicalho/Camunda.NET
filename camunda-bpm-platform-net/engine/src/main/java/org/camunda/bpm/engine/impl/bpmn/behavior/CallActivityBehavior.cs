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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using MigratingActivityInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingActivityInstance;
	using MigratingCalledProcessInstance = org.camunda.bpm.engine.impl.migration.instance.MigratingCalledProcessInstance;
	using MigratingInstanceParseContext = org.camunda.bpm.engine.impl.migration.instance.parser.MigratingInstanceParseContext;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using PvmProcessInstance = org.camunda.bpm.engine.impl.pvm.PvmProcessInstance;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using MigrationObserverBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.MigrationObserverBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.CallableElementUtil.getProcessDefinitionToCall;


	/// <summary>
	/// Implementation of the BPMN 2.0 call activity
	/// (limited currently to calling a subprocess and not (yet) a global task).
	/// 
	/// @author Joram Barrez
	/// @author Roman Smirnov
	/// </summary>
	public class CallActivityBehavior : CallableElementActivityBehavior, MigrationObserverBehavior
	{

	  public CallActivityBehavior()
	  {
	  }

	  public CallActivityBehavior(string className) : base(className)
	  {
	  }

	  public CallActivityBehavior(Expression expression) : base(expression)
	  {
	  }

	  protected internal override void startInstance(ActivityExecution execution, VariableMap variables, string businessKey)
	  {
		ProcessDefinitionImpl definition = getProcessDefinitionToCall(execution, CallableElement);
		PvmProcessInstance processInstance = execution.createSubProcessInstance(definition, businessKey);
		processInstance.start(variables);
	  }

	  public virtual void migrateScope(ActivityExecution scopeExecution)
	  {
	  }

	  public virtual void onParseMigratingInstance(MigratingInstanceParseContext parseContext, MigratingActivityInstance migratingInstance)
	  {
		ActivityImpl callActivity = (ActivityImpl) migratingInstance.SourceScope;

		// A call activity is typically scope and since we guarantee stability of scope executions during migration,
		// the superExecution link does not have to be maintained during migration.
		// There are some exceptions, though: A multi-instance call activity is not scope and therefore
		// does not have a dedicated scope execution. In this case, the link to the super execution
		// must be maintained throughout migration
		if (!callActivity.Scope)
		{
		  ExecutionEntity callActivityExecution = migratingInstance.resolveRepresentativeExecution();
		  ExecutionEntity calledProcessInstance = callActivityExecution.getSubProcessInstance();
		  migratingInstance.addMigratingDependentInstance(new MigratingCalledProcessInstance(calledProcessInstance));
		}
	  }

	}

}