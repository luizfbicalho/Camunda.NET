using System;

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
namespace org.camunda.bpm.engine.history
{

	/// <summary>
	/// A single field that was submitted in either a start form or a task form.
	/// This is the audit information that can be used to trace who supplied which
	/// input for which tasks at what time.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	[Obsolete]
	public interface HistoricFormProperty : HistoricDetail
	{

	  /// <summary>
	  /// the id or key of the property </summary>
	  string PropertyId {get;}

	  /// <summary>
	  /// the submitted value </summary>
	  string PropertyValue {get;}

	}

}