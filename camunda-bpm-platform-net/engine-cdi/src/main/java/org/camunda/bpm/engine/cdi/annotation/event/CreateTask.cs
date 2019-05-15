﻿/*
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
namespace org.camunda.bpm.engine.cdi.annotation.@event
{

	/// <summary>
	/// Can be used to qualify events fired when a task is created.
	/// 
	/// <pre>
	/// public void onApproveRegistrationTaskCreate(@Observes @CreateTask("approveRegistration") BusinessProcessEvent evt) {
	///   // ...
	/// }
	/// </pre>
	/// 
	/// @author Daniel Meyer
	/// @author Sebastian Menski
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Retention(RetentionPolicy.RUNTIME) @Qualifier public class CreateTask extends System.Attribute
	public class CreateTask : System.Attribute
	{
	  /// <summary>
	  /// the definition key (id of the task in BPMN XML) of the task which was created </summary>
	  public string value;

		public CreateTask(public String value)
		{
			this.value = value;
		}
	}

}