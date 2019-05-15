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
namespace org.camunda.bpm.engine.test.cmmn.handler
{

	using CaseExecutionListener = org.camunda.bpm.engine.@delegate.CaseExecutionListener;
	using ItemHandler = org.camunda.bpm.engine.impl.cmmn.handler.ItemHandler;
	using AbstractExecutionListenerSpec = org.camunda.bpm.engine.test.cmmn.handler.specification.AbstractExecutionListenerSpec;
	using ClassExecutionListenerSpec = org.camunda.bpm.engine.test.cmmn.handler.specification.ClassExecutionListenerSpec;
	using DelegateExpressionExecutionListenerSpec = org.camunda.bpm.engine.test.cmmn.handler.specification.DelegateExpressionExecutionListenerSpec;
	using ExpressionExecutionListenerSpec = org.camunda.bpm.engine.test.cmmn.handler.specification.ExpressionExecutionListenerSpec;
	using ScriptExecutionListenerSpec = org.camunda.bpm.engine.test.cmmn.handler.specification.ScriptExecutionListenerSpec;

	public class ExecutionListenerCases
	{

	  public static final IEnumerable<object[]> TASK_OR_STAGE_CASES = Arrays.asList(new object[][] { {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.ENABLE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ENABLE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.START)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.MANUAL_START)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.EXIT)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_SUSPEND)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_RESUME)}, {new ClassExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)
				  .expectRegistrationFor(ItemHandler.TASK_OR_STAGE_EVENTS)
	}
	,

	{
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldExpression("firstField", "${myFirstExpression}").withFieldExpression("secondField", "${mySecondExpression}")
	}
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildExpression("firstField", "${myFirstExpression}").withFieldChildExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldStringValue("firstField", "aFirstFixedValue").withFieldStringValue("secondField", "aSecondFixedValue")
			 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildStringValue("firstField", "aFirstFixedValue").withFieldChildStringValue("secondField", "aSecondFixedValue")
			 }
			 ,

			 {
			  // script
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.ENABLE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ENABLE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.START)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.MANUAL_START)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.EXIT)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_SUSPEND)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_RESUME)
			 }
			 ,
			 {
				  (new ScriptExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.TASK_OR_STAGE_EVENTS)
			 }
			 ,

			 {
			  // delegate expression
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.ENABLE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ENABLE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.START)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.MANUAL_START)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.EXIT)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_SUSPEND)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_RESUME)
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.TASK_OR_STAGE_EVENTS)
			 }
			 ,

			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldExpression("firstField", "${myFirstExpression}").withFieldExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildExpression("firstField", "${myFirstExpression}").withFieldChildExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldStringValue("firstField", "aFirstFixedValue").withFieldStringValue("secondField", "aSecondFixedValue")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildStringValue("firstField", "aFirstFixedValue").withFieldChildStringValue("secondField", "aSecondFixedValue")
			 }
			 ,

			 {
			  // expression
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.ENABLE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.DISABLE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ENABLE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.START)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.MANUAL_START)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.EXIT)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_SUSPEND)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_RESUME)
			 }
			 ,
			 {
				  (new ExpressionExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.TASK_OR_STAGE_EVENTS)}
			 }
		 );


	  public static final IEnumerable<object[]> EVENTLISTENER_OR_MILESTONE_CASES = Arrays.asList(new object[][] { {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_TERMINATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.OCCUR)}, {new ClassExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)
				  .expectRegistrationFor(ItemHandler.EVENT_LISTENER_OR_MILESTONE_EVENTS)
}
				 ,

				 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldExpression("firstField", "${myFirstExpression}").withFieldExpression("secondField", "${mySecondExpression}")
				 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildExpression("firstField", "${myFirstExpression}").withFieldChildExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldStringValue("firstField", "aFirstFixedValue").withFieldStringValue("secondField", "aSecondFixedValue")
			 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildStringValue("firstField", "aFirstFixedValue").withFieldChildStringValue("secondField", "aSecondFixedValue")
			 }
			 ,

			 {
			  // script
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_TERMINATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.OCCUR)
			 }
			 ,
			 {
				  (new ScriptExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.EVENT_LISTENER_OR_MILESTONE_EVENTS)
			 }
			 ,

			 {
			  // delegate expression
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_TERMINATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.OCCUR)
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.EVENT_LISTENER_OR_MILESTONE_EVENTS)
			 }
			 ,

			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldExpression("firstField", "${myFirstExpression}").withFieldExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildExpression("firstField", "${myFirstExpression}").withFieldChildExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldStringValue("firstField", "aFirstFixedValue").withFieldStringValue("secondField", "aSecondFixedValue")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildStringValue("firstField", "aFirstFixedValue").withFieldChildStringValue("secondField", "aSecondFixedValue")
			 }
			 ,

			 {
			  // expression
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RESUME)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.PARENT_TERMINATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.OCCUR)
			 }
			 ,
			 {
				  (new ExpressionExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.EVENT_LISTENER_OR_MILESTONE_EVENTS)
			 }
			 ,
		  }
		 );

	  public static final IEnumerable<object[]> CASE_PLAN_MODEL_CASES = Arrays.asList(new object[][] { {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ACTIVATE)}, {new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CLOSE)}, {new ClassExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)
				  .expectRegistrationFor(ItemHandler.CASE_PLAN_MODEL_EVENTS)
				  }
				 ,

				 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldExpression("firstField", "${myFirstExpression}").withFieldExpression("secondField", "${mySecondExpression}")
				 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildExpression("firstField", "${myFirstExpression}").withFieldChildExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldStringValue("firstField", "aFirstFixedValue").withFieldStringValue("secondField", "aSecondFixedValue")
			 }
			 ,
			 {
				  (new ClassExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildStringValue("firstField", "aFirstFixedValue").withFieldChildStringValue("secondField", "aSecondFixedValue")
			 }
			 ,

			 {
			  // script
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ACTIVATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new ScriptExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CLOSE)
			 }
			 ,
			 {
				  (new ScriptExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.CASE_PLAN_MODEL_EVENTS)
			 }
			 ,

			 {
			  // delegate expression
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ACTIVATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CLOSE)
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.CASE_PLAN_MODEL_EVENTS)
			 }
			 ,

			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldExpression("firstField", "${myFirstExpression}").withFieldExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildExpression("firstField", "${myFirstExpression}").withFieldChildExpression("secondField", "${mySecondExpression}")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldStringValue("firstField", "aFirstFixedValue").withFieldStringValue("secondField", "aSecondFixedValue")
			 }
			 ,
			 {
				  (new DelegateExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)).withFieldChildStringValue("firstField", "aFirstFixedValue").withFieldChildStringValue("secondField", "aSecondFixedValue")
			 }
			 ,

			 {
			  // expression
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CREATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.RE_ACTIVATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.SUSPEND)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.TERMINATE)
			 }
			 ,
			 {
				  new ExpressionExecutionListenerSpec(org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.CLOSE)
			 }
			 ,
			 {
				  (new ExpressionExecutionListenerSpec(AbstractExecutionListenerSpec.ANY_EVENT)).expectRegistrationFor(ItemHandler.CASE_PLAN_MODEL_EVENTS)}
			 }
		 );


	}

}