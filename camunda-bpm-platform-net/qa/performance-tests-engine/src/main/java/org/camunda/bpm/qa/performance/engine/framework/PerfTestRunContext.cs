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
	/// <summary>
	/// Represents an individual run. This is passed from one step to
	/// another and can be used for passing context data.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface PerfTestRunContext
	{

	  void setVariable(string name, object value);

	  T getVariable<T>(string name);

	}

	public static class PerfTestRunContext_Fields
	{
	  public static readonly ThreadLocal<PerfTestRunContext> currentContext = new ThreadLocal<PerfTestRunContext>();
	}

}