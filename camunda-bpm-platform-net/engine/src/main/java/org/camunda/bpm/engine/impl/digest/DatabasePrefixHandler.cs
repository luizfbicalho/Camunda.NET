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
namespace org.camunda.bpm.engine.impl.digest
{

	/// <summary>
	/// In order to distinguish between the used hashed algorithm
	/// for the password encryption, as prefix is persisted with the
	/// encrypted to the database.
	/// The <seealso cref="DatabasePrefixHandler"/> is used to handle the prefix, especially for building
	/// the prefix, retrieving the algorithm name from the prefix and
	/// removing the prefix name from the hashed password.
	/// </summary>
	public class DatabasePrefixHandler
	{

	  protected internal Pattern pattern = Pattern.compile("^\\{(.*?)\\}");

	  public virtual string generatePrefix(string algorithmName)
	  {
		return "{" + algorithmName + "}";
	  }

	  public virtual string retrieveAlgorithmName(string encryptedPasswordWithPrefix)
	  {
		Matcher matcher = pattern.matcher(encryptedPasswordWithPrefix);
		if (matcher.find())
		{
		  return matcher.group(1);
		}
		return null;
	  }

	  public virtual string removePrefix(string encryptedPasswordWithPrefix)
	  {
		int index = encryptedPasswordWithPrefix.IndexOf("}", StringComparison.Ordinal);
		if (!encryptedPasswordWithPrefix.StartsWith("{", StringComparison.Ordinal) || index < 0)
		{
		  return null;
		}
		return encryptedPasswordWithPrefix.Substring(index + 1);
	  }

	}

}