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
	using ActivityStartBehavior = org.camunda.bpm.engine.impl.pvm.process.ActivityStartBehavior;


	/// <summary>
	/// <para>The BPMN Boundary Event.</para>
	/// 
	/// <para>The behavior of the boundary event is defined via it's <seealso cref="ActivityStartBehavior"/>. It must be either
	/// {@value ActivityStartBehavior#CANCEL_EVENT_SCOPE} or {@value ActivityStartBehavior#CONCURRENT_IN_FLOW_SCOPE} meaning
	/// that it will either cancel the scope execution for the activity it is attached to (it's event scope) or will be executed concurrently
	/// in it's flow scope.</para>
	/// <para>The boundary event does noting "special" in its inner behavior.</para>
	/// 
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// </summary>
	public class BoundaryEventActivityBehavior : FlowNodeActivityBehavior
	{

	}

}