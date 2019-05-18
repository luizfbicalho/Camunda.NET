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
namespace org.camunda.bpm.model.bpmn.instance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.model.bpmn.impl.BpmnModelConstants.CAMUNDA_NS;


	using Supports = org.camunda.bpm.model.bpmn.impl.instance.Supports;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class ProcessTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(CallableElement), false);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(Auditing), 0, 1), new ChildElementAssumption(typeof(Monitoring), 0, 1), new ChildElementAssumption(typeof(Property)), new ChildElementAssumption(typeof(LaneSet)), new ChildElementAssumption(typeof(FlowElement)), new ChildElementAssumption(typeof(Artifact)), new ChildElementAssumption(typeof(ResourceRole)), new ChildElementAssumption(typeof(CorrelationSubscription)), new ChildElementAssumption(typeof(Supports))
		   );
		  }
	  }

	  public override ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("processType", false, false, ProcessType.None), new AttributeAssumption("isClosed", false, false, false), new AttributeAssumption("isExecutable"), new AttributeAssumption(CAMUNDA_NS, "candidateStarterGroups"), new AttributeAssumption(CAMUNDA_NS, "candidateStarterUsers"), new AttributeAssumption(CAMUNDA_NS, "jobPriority"), new AttributeAssumption(CAMUNDA_NS, "taskPriority"), new AttributeAssumption(CAMUNDA_NS, "historyTimeToLive"), new AttributeAssumption(CAMUNDA_NS, "isStartableInTasklist", false, false, true), new AttributeAssumption(CAMUNDA_NS, "versionTag")
		   );
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaJobPriority()
	  public virtual void testCamundaJobPriority()
	  {
		Process process = modelInstance.newInstance(typeof(Process));
		assertThat(process.CamundaJobPriority).Null;

		process.CamundaJobPriority = "15";

		assertThat(process.CamundaJobPriority).isEqualTo("15");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaTaskPriority()
	  public virtual void testCamundaTaskPriority()
	  {
		//given
		Process proc = modelInstance.newInstance(typeof(Process));
		assertThat(proc.CamundaTaskPriority).Null;
		//when
		proc.CamundaTaskPriority = BpmnTestConstants.TEST_PROCESS_TASK_PRIORITY;
		//then
		assertThat(proc.CamundaTaskPriority).isEqualTo(BpmnTestConstants.TEST_PROCESS_TASK_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCamundaHistoryTimeToLive()
	  public virtual void testCamundaHistoryTimeToLive()
	  {
		//given
		Process proc = modelInstance.newInstance(typeof(Process));
		assertThat(proc.getCamundaHistoryTimeToLive()).Null;
		//when
		proc.setCamundaHistoryTimeToLive(BpmnTestConstants.TEST_HISTORY_TIME_TO_LIVE);
		//then
		assertThat(proc.getCamundaHistoryTimeToLive()).isEqualTo(BpmnTestConstants.TEST_HISTORY_TIME_TO_LIVE);
	  }
	}

}