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
namespace org.camunda.bpm.engine.impl.cfg
{


	/// <summary>
	/// The transaction context is an abstraction for different transaction management strategies
	/// existing the Java Ecosystem. Provides transaction lifecycle management and management of transaction listeners.
	/// 
	/// Note: not every Technology or environment may provide a full implementation of this interface.
	/// 
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public interface TransactionContext
	{

	  /// <summary>
	  /// Commit the current transaction.
	  /// </summary>
	  void commit();

	  /// <summary>
	  /// Rollback the current transaction.
	  /// </summary>
	  void rollback();


	  /// <summary>
	  /// Add a <seealso cref="TransactionListener"/> to the current transaction.
	  /// </summary>
	  /// <param name="transactionState"> the transaction state for which the <seealso cref="TransactionListener"/> should be added. </param>
	  /// <param name="transactionListener"> the <seealso cref="TransactionListener"/> to add. </param>
	  void addTransactionListener(TransactionState transactionState, TransactionListener transactionListener);

	  bool TransactionActive {get;}

	}

}