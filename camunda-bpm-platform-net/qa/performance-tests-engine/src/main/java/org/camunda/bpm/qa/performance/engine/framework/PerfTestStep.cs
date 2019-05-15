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
namespace org.camunda.bpm.qa.performance.engine.framework
{
	using WaitStep = org.camunda.bpm.qa.performance.engine.steps.WaitStep;

	/// <summary>
	/// A step in a performance test.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestStep
	{

	  protected internal readonly PerfTestStepBehavior perfTestStepBehavior;

	  protected internal PerfTestStep nextStep;

	  protected internal string stepName;

	  public PerfTestStep(PerfTestStepBehavior behavior)
	  {
		perfTestStepBehavior = behavior;
	  }

	  public virtual PerfTestStep NextStep
	  {
		  set
		  {
			nextStep = value;
		  }
		  get
		  {
			return nextStep;
		  }
	  }

	  public virtual PerfTestStepBehavior StepBehavior
	  {
		  get
		  {
			return perfTestStepBehavior;
		  }
	  }


	  public virtual string StepName
	  {
		  get
		  {
			return perfTestStepBehavior.GetType().Name;
		  }
	  }

	  public virtual bool WaitStep
	  {
		  get
		  {
			return perfTestStepBehavior is WaitStep;
		  }
	  }

	}

}