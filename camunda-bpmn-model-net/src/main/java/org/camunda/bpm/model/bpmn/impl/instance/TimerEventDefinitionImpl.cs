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
namespace org.camunda.bpm.model.bpmn.impl.instance
{
	using org.camunda.bpm.model.bpmn.instance;
	using ModelBuilder = org.camunda.bpm.model.xml.ModelBuilder;
	using ModelTypeInstanceContext = org.camunda.bpm.model.xml.impl.instance.ModelTypeInstanceContext;
	using ModelElementTypeBuilder = org.camunda.bpm.model.xml.type.ModelElementTypeBuilder;
	using ChildElement = org.camunda.bpm.model.xml.type.child.ChildElement;
	using SequenceBuilder = org.camunda.bpm.model.xml.type.child.SequenceBuilder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN20_NS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.BPMN_ELEMENT_TIMER_EVENT_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.xml.type.ModelElementTypeBuilder.ModelTypeInstanceProvider;

	/// <summary>
	/// The BPMN timerEventDefinition element
	/// 
	/// @author Sebastian Menski
	/// </summary>
	public class TimerEventDefinitionImpl : EventDefinitionImpl, TimerEventDefinition
	{

	  protected internal static ChildElement<TimeDate> timeDateChild;
	  protected internal static ChildElement<TimeDuration> timeDurationChild;
	  protected internal static ChildElement<TimeCycle> timeCycleChild;

	  public static void registerType(ModelBuilder modelBuilder)
	  {
		ModelElementTypeBuilder typeBuilder = modelBuilder.defineType(typeof(TimerEventDefinition), BPMN_ELEMENT_TIMER_EVENT_DEFINITION).namespaceUri(BPMN20_NS).extendsType(typeof(EventDefinition)).instanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		SequenceBuilder sequenceBuilder = typeBuilder.sequence();

		timeDateChild = sequenceBuilder.element(typeof(TimeDate)).build();

		timeDurationChild = sequenceBuilder.element(typeof(TimeDuration)).build();

		timeCycleChild = sequenceBuilder.element(typeof(TimeCycle)).build();

		typeBuilder.build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : ModelTypeInstanceProvider<TimerEventDefinition>
	  {
		  public TimerEventDefinition newInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new TimerEventDefinitionImpl(instanceContext);
		  }
	  }

	  public TimerEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
	  {
	  }

	  public virtual TimeDate TimeDate
	  {
		  get
		  {
			return timeDateChild.getChild(this);
		  }
		  set
		  {
			timeDateChild.setChild(this, value);
		  }
	  }


	  public virtual TimeDuration TimeDuration
	  {
		  get
		  {
			return timeDurationChild.getChild(this);
		  }
		  set
		  {
			timeDurationChild.setChild(this, value);
		  }
	  }


	  public virtual TimeCycle TimeCycle
	  {
		  get
		  {
			return timeCycleChild.getChild(this);
		  }
		  set
		  {
			timeCycleChild.setChild(this, value);
		  }
	  }


	}

}