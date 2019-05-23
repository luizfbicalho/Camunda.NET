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
namespace org.camunda.bpm.model.xml
{

	/// <summary>
	/// Exception thrown if a user attempts to perform an unsupported
	/// model operation. Example: call <seealso cref="Collection.add(object)"/> on an
	/// immutable collection.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class UnsupportedModelOperationException : System.NotSupportedException
	{

	  private const long serialVersionUID = 1L;

	  public UnsupportedModelOperationException(string operationName, string reason) : base("The operation " + operationName + " is unsupported: " + reason + ".")
	  {
	  }

	}

}