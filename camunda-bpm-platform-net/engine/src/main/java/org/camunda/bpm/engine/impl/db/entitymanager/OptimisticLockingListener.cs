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
namespace org.camunda.bpm.engine.impl.db.entitymanager
{
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using DbOperationType = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperationType;

	/// <summary>
	/// Allows registering a listener which is notified when an
	/// <seealso cref="DbOperationType#UPDATE"/> or <seealso cref="DbOperationType#DELETE"/>
	/// could not be performed.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface OptimisticLockingListener
	{

	  /// <summary>
	  /// The type of the entity for which this listener should be notified.
	  /// If the implementation returns 'null', the listener is notified for all
	  /// entity types.
	  /// </summary>
	  /// <returns> the entity type for which the listener should be notified. </returns>
	  Type EntityType {get;}

	  /// <summary>
	  /// Signifies that an operation failed due to optimistic locking.
	  /// </summary>
	  /// <param name="operation"> the failed operation. </param>
	  void failedOperation(DbOperation operation);

	}

}