using System.Text;

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

	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class Base64EncodedHashDigest
	{

	  public virtual string encrypt(string password)
	  {

		// create hash as byte array
		sbyte[] hash = createByteHash(password);

		// stringify hash (default implementation use BASE64 encoding)
		return encodeHash(hash);

	  }

	  public virtual bool check(string password, string encrypted)
	  {
		return encrypt(password).Equals(encrypted);
	  }

	  protected internal virtual sbyte[] createByteHash(string password)
	  {
		MessageDigest digest = createDigestInstance();
		try
		{
		  digest.update(password.GetBytes(Encoding.UTF8));
		  return digest.digest();

		}
		catch (UnsupportedEncodingException)
		{
		  throw new ProcessEngineException("UnsupportedEncodingException while calculating password digest");
		}
	  }

	  protected internal virtual MessageDigest createDigestInstance()
	  {
		try
		{
		  return MessageDigest.getInstance(hashAlgorithmName());

		}
		catch (NoSuchAlgorithmException)
		{
		  throw new ProcessEngineException("Cannot lookup " + hashAlgorithmName() + " algorithm");

		}
	  }

	  protected internal virtual string encodeHash(sbyte[] hash)
	  {
		return StringHelper.NewString(Base64.encodeBase64(hash));
	  }

	  /// <summary>
	  /// allows subclasses to select the hash algorithm </summary>
	  protected internal abstract string hashAlgorithmName();

	}

}