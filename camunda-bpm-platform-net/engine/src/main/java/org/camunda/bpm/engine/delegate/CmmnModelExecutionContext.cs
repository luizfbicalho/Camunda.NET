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
namespace org.camunda.bpm.engine.@delegate
{
	using CmmnModelInstance = org.camunda.bpm.model.cmmn.CmmnModelInstance;
	using CmmnElement = org.camunda.bpm.model.cmmn.instance.CmmnElement;

	/// <summary>
	/// Implemented by classes which provide access to the <seealso cref="CmmnModelInstance"/>
	/// and the currently executed <seealso cref="CmmnElement"/>.
	/// 
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CmmnModelExecutionContext
	{

	  /// <summary>
	  /// Returns the <seealso cref="CmmnModelInstance"/> for the currently executed Cmmn Model
	  /// </summary>
	  /// <returns> the current <seealso cref="CmmnModelInstance"/> </returns>
	  CmmnModelInstance CmmnModelInstance {get;}

	  /// <summary>
	  /// <para>Returns the currently executed Element in the Cmmn Model. This method returns a <seealso cref="CmmnElement"/> which may be casted
	  /// to the concrete type of the Cmmn Model Element currently executed.</para>
	  /// </summary>
	  /// <returns> the <seealso cref="CmmnElement"/> corresponding to the current Cmmn Model Element </returns>
	  CmmnElement CmmnModelElementInstance {get;}

	}

}