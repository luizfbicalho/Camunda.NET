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
namespace org.camunda.bpm.identity.impl.ldap.util
{



	public class CertificateHelper
	{

	  public static void acceptUntrusted()
	  {
		try
		{
		  SSLContext sslContext = SSLContext.getInstance("TLS");
		  sslContext.init(new KeyManager[0], new TrustManager[] {new DefaultTrustManager()}, new SecureRandom());
		  SSLContext.Default = sslContext;
		}
		catch (Exception ex)
		{
		  throw new Exception("Could not change SSL TrustManager to accept arbitrary certificates", ex);
		}
	  }

	  private class DefaultTrustManager : X509TrustManager
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void checkClientTrusted(java.security.cert.X509Certificate[] arg0, String arg1) throws java.security.cert.CertificateException
		public override void checkClientTrusted(X509Certificate[] arg0, string arg1)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void checkServerTrusted(java.security.cert.X509Certificate[] arg0, String arg1) throws java.security.cert.CertificateException
		public override void checkServerTrusted(X509Certificate[] arg0, string arg1)
		{
		}

		public override X509Certificate[] AcceptedIssuers
		{
			get
			{
			  return null;
			}
		}

	  }
	}

}