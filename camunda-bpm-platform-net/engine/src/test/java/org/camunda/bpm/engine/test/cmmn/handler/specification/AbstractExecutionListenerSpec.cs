using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.test.cmmn.handler.specification
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using BaseDelegateExecution = org.camunda.bpm.engine.@delegate.BaseDelegateExecution;
	using DelegateListener = org.camunda.bpm.engine.@delegate.DelegateListener;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using CmmnModelElementInstance = org.camunda.bpm.model.cmmn.instance.CmmnModelElementInstance;
	using ExtensionElements = org.camunda.bpm.model.cmmn.instance.ExtensionElements;
	using CamundaCaseExecutionListener = org.camunda.bpm.model.cmmn.instance.camunda.CamundaCaseExecutionListener;

	public abstract class AbstractExecutionListenerSpec
	{

	  public const string ANY_EVENT = "any";

	  protected internal string eventNameToRegisterOn;
	  protected internal ISet<string> expectedRegisteredEvents;

	  protected internal IList<FieldSpec> fieldSpecs;

	  public AbstractExecutionListenerSpec(string eventName)
	  {
		this.eventNameToRegisterOn = eventName;
		this.expectedRegisteredEvents = new HashSet<string>();
		this.expectedRegisteredEvents.Add(eventName);

		this.fieldSpecs = new List<FieldSpec>();
	  }

	  public virtual void addListenerToElement(CmmnModelInstance modelInstance, CmmnModelElementInstance modelElement)
	  {
		ExtensionElements extensionElements = SpecUtil.createElement(modelInstance, modelElement, null, typeof(ExtensionElements));
		CamundaCaseExecutionListener caseExecutionListener = SpecUtil.createElement(modelInstance, extensionElements, null, typeof(CamundaCaseExecutionListener));

		if (!ANY_EVENT.Equals(eventNameToRegisterOn))
		{
		  caseExecutionListener.CamundaEvent = eventNameToRegisterOn;
		}

		configureCaseExecutionListener(modelInstance, caseExecutionListener);

		foreach (FieldSpec fieldSpec in fieldSpecs)
		{
		  fieldSpec.addFieldToListenerElement(modelInstance, caseExecutionListener);
		}
	  }

	  protected internal abstract void configureCaseExecutionListener(CmmnModelInstance modelInstance, CamundaCaseExecutionListener listener);

	  public virtual void verify(CmmnActivity activity)
	  {

		assertEquals(expectedRegisteredEvents.Count, activity.Listeners.Count);

		foreach (string expectedRegisteredEvent in expectedRegisteredEvents)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.delegate.DelegateListener<? extends org.camunda.bpm.engine.delegate.BaseDelegateExecution>> listeners = activity.getListeners(expectedRegisteredEvent);
		  IList<DelegateListener<BaseDelegateExecution>> listeners = activity.getListeners(expectedRegisteredEvent);
		  assertEquals(1, listeners.Count);
		  verifyListener(listeners[0]);
		}
	  }

	  protected internal abstract void verifyListener<T1>(DelegateListener<T1> listener) where T1 : org.camunda.bpm.engine.@delegate.BaseDelegateExecution;

	  public virtual AbstractExecutionListenerSpec expectRegistrationFor(IList<string> events)
	  {
		expectedRegisteredEvents = new HashSet<string>(events);
		return this;
	  }

	  public virtual AbstractExecutionListenerSpec withFieldExpression(string fieldName, string expression)
	  {
		fieldSpecs.Add(new FieldSpec(fieldName, expression, null, null, null));
		return this;
	  }

	  public virtual AbstractExecutionListenerSpec withFieldChildExpression(string fieldName, string expression)
	  {
		fieldSpecs.Add(new FieldSpec(fieldName, null, expression, null, null));
		return this;
	  }

	  public virtual AbstractExecutionListenerSpec withFieldStringValue(string fieldName, string value)
	  {
		fieldSpecs.Add(new FieldSpec(fieldName, null, null, value, null));
		return this;
	  }

	  public virtual AbstractExecutionListenerSpec withFieldChildStringValue(string fieldName, string value)
	  {
		fieldSpecs.Add(new FieldSpec(fieldName, null, null, null, value));
		return this;
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();

		sb.Append("{type=");
		sb.Append(this.GetType().Name);
		sb.Append(", event=");
		sb.Append(eventNameToRegisterOn);
		sb.Append("}");

		return sb.ToString();
	  }
	}

}