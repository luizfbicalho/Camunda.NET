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
namespace org.camunda.bpm.engine.test.standalone.deploy
{
	using CompensationEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.CompensationEventActivityBehavior;
	using NoneEndEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.NoneEndEventActivityBehavior;
	using NoneStartEventActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.NoneStartEventActivityBehavior;
	using AbstractBpmnParseListener = org.camunda.bpm.engine.impl.bpmn.parser.AbstractBpmnParseListener;
	using CompensateEventDefinition = org.camunda.bpm.engine.impl.bpmn.parser.CompensateEventDefinition;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;
	using Element = org.camunda.bpm.engine.impl.util.xml.Element;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse.COMPENSATE_EVENT_DEFINITION;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class TestBPMNParseListener : AbstractBpmnParseListener
	{

	  public override void parseRootElement(Element rootElement, IList<ProcessDefinitionEntity> processDefinitions)
	  {
		// Change the key of all deployed process-definitions
		foreach (ProcessDefinitionEntity entity in processDefinitions)
		{
		  entity.Key = entity.Key + "-modified";
		}
	  }

	  public override void parseStartEvent(Element startEventElement, ScopeImpl scope, ActivityImpl startEventActivity)
	  {
		// Change activity behavior
		startEventActivity.ActivityBehavior = new TestNoneStartEventActivityBehavior(this);
	  }

	  public override void parseIntermediateThrowEvent(Element intermediateEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		// Change activity behavior
		Element compensateEventDefinitionElement = intermediateEventElement.element(COMPENSATE_EVENT_DEFINITION);
		if (compensateEventDefinitionElement != null)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String activityRef = compensateEventDefinitionElement.attribute("activityRef");
		  string activityRef = compensateEventDefinitionElement.attribute("activityRef");
		  CompensateEventDefinition compensateEventDefinition = new CompensateEventDefinition();
		  compensateEventDefinition.ActivityRef = activityRef;
		  compensateEventDefinition.WaitForCompletion = false;

		  activity.ActivityBehavior = new TestCompensationEventActivityBehavior(this, compensateEventDefinition);
		}
	  }

	  public override void parseEndEvent(Element endEventElement, ScopeImpl scope, ActivityImpl activity)
	  {
		// Change activity behavior
		activity.ActivityBehavior = new TestNoneEndEventActivityBehavior(this);
	  }

	  public class TestNoneStartEventActivityBehavior : NoneStartEventActivityBehavior
	  {
		  private readonly TestBPMNParseListener outerInstance;

		  public TestNoneStartEventActivityBehavior(TestBPMNParseListener outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


	  }

	  public class TestNoneEndEventActivityBehavior : NoneEndEventActivityBehavior
	  {
		  private readonly TestBPMNParseListener outerInstance;

		  public TestNoneEndEventActivityBehavior(TestBPMNParseListener outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


	  }

	  public class TestCompensationEventActivityBehavior : CompensationEventActivityBehavior
	  {
		  private readonly TestBPMNParseListener outerInstance;


		public TestCompensationEventActivityBehavior(TestBPMNParseListener outerInstance, CompensateEventDefinition compensateEventDefinition) : base(compensateEventDefinition)
		{
			this.outerInstance = outerInstance;
		}
	  }

	}

}