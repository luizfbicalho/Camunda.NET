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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
	using AbstractSetJobDefinitionStateCmd = org.camunda.bpm.engine.impl.cmd.AbstractSetJobDefinitionStateCmd;
	using SuspendJobDefinitionCmd = org.camunda.bpm.engine.impl.cmd.SuspendJobDefinitionCmd;

	/// <summary>
	/// @author roman.smirnov
	/// </summary>
	public class TimerSuspendJobDefinitionHandler : TimerChangeJobDefinitionSuspensionStateJobHandler
	{

	  public const string TYPE = "suspend-job-definition";

	  public override string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  protected internal override AbstractSetJobDefinitionStateCmd getCommand(JobDefinitionSuspensionStateConfiguration configuration)
	  {

		return new SuspendJobDefinitionCmd(configuration.createBuilder());
	  }

	}

}