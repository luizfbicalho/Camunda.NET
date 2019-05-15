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
namespace org.camunda.bpm.engine.impl.digest
{

	/// <summary>
	/// The <seealso cref="PasswordEncryptor"/> provides the api to customize
	/// the encryption of passwords.
	/// 
	/// @author Daniel Meyer
	/// @author nico.rehwaldt
	/// </summary>
	public interface PasswordEncryptor
	{

	  /// <summary>
	  /// Encrypt the given password
	  /// </summary>
	  /// <param name="password">
	  /// @return </param>
	  string encrypt(string password);

	  /// <summary>
	  /// Returns true if the given plain text equals to the encrypted password.
	  /// </summary>
	  /// <param name="password"> </param>
	  /// <param name="encrypted">
	  /// 
	  /// @return </param>
	  bool check(string password, string encrypted);

	  /// <summary>
	  /// In order to distinguish which algorithm was used to hash the
	  /// password, it needs a unique id. In particular, this is needed
	  /// for <seealso cref="#check"/>.
	  /// </summary>
	  /// <returns> the name of the algorithm </returns>
	  string hashAlgorithmName();
	}

}