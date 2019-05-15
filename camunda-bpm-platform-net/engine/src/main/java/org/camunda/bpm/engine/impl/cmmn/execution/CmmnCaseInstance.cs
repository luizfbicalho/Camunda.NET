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
namespace org.camunda.bpm.engine.impl.cmmn.execution
{

	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface CmmnCaseInstance : CmmnActivityExecution
	{

	  /// <summary>
	  /// <para><code>This</code> case instance transitions to <code>ACTIVE</code>
	  /// state.</para>
	  /// </summary>
	  void create();

	  /// <summary>
	  /// <para><code>This</code> case instance transitions to <code>ACTIVE</code>
	  /// state.</para>
	  /// 
	  /// <para>The given <code>variables</code> will be set a case instance variables.</para>
	  /// </summary>
	  void create(IDictionary<string, object> variables);

	  /// <summary>
	  /// <para>Find a case execution by the given <code>activityId</code>.</para>
	  /// </summary>
	  /// <param name="activityId"> the id of the <seealso cref="CmmnActivity activity"/> to
	  ///                   which a case execution is associated.
	  /// </param>
	  /// <returns> returns a case execution or null if a case execution could
	  ///         not be found. </returns>
	  CmmnActivityExecution findCaseExecution(string activityId);

	}

}