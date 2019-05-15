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
namespace org.camunda.bpm.engine.impl.core.operation
{
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Roman Smirnov
	/// @author Sebastian Menski
	/// </summary>
	/// @param <T> The execution type this atomic operation should work on. </param>
	public interface CoreAtomicOperation<T> where T : org.camunda.bpm.engine.impl.core.instance.CoreExecution
	{

	  void execute(T instance);

	  bool isAsync(T instance);

	  string CanonicalName {get;}

	}

}