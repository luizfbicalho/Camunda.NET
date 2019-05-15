using System;
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
namespace org.camunda.bpm.container.impl.jmx.deployment.util
{
	using ProcessApplicationScanningUtil = org.camunda.bpm.container.impl.deployment.scanning.ProcessApplicationScanningUtil;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	/// <summary>
	/// @author Clint Manning
	/// </summary>
	public class VfsProcessApplicationScannerTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanProcessArchivePathForResources() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanProcessArchivePathForResources()
	  {

		// given: scanning the relative test resource root
		URLClassLoader classLoader = new URLClassLoader(new URL[]{new URL("file:")});
		string processRootPath = "classpath:org/camunda/bpm/container/impl/jmx/deployment/process/";
		IDictionary<string, sbyte[]> scanResult = ProcessApplicationScanningUtil.findResources(classLoader, processRootPath, null);

		// expect: finds only the BPMN process file and not treats the 'bpmn' folder
		assertEquals(1, scanResult.Count);
		string processFileName = "VfsProcessScannerTest.bpmn20.xml";
		assertTrue("'" + processFileName + "'not found", contains(scanResult, processFileName));
		assertFalse("'bpmn' folder in resource path found", contains(scanResult, "processResource.txt"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanProcessArchivePathForCmmnResources() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanProcessArchivePathForCmmnResources()
	  {

		// given: scanning the relative test resource root
		URLClassLoader classLoader = new URLClassLoader(new URL[]{new URL("file:")});
		string processRootPath = "classpath:org/camunda/bpm/container/impl/jmx/deployment/case/";
		IDictionary<string, sbyte[]> scanResult = ProcessApplicationScanningUtil.findResources(classLoader, processRootPath, null);

		// expect: finds only the CMMN process file and not treats the 'cmmn' folder
		assertEquals(1, scanResult.Count);
		string processFileName = "VfsProcessScannerTest.cmmn";
		assertTrue("'" + processFileName + "' not found", contains(scanResult, processFileName));
		assertFalse("'cmmn' in resource path found", contains(scanResult, "caseResource.txt"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanProcessArchivePathWithAdditionalResourceSuffixes() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanProcessArchivePathWithAdditionalResourceSuffixes()
	  {
		URLClassLoader classLoader = new URLClassLoader(new URL[]{new URL("file:")});
		string processRootPath = "classpath:org/camunda/bpm/container/impl/jmx/deployment/script/";
		string[] additionalResourceSuffixes = new string[] {"py", "groovy", "rb"};
		IDictionary<string, sbyte[]> scanResult = ProcessApplicationScanningUtil.findResources(classLoader, processRootPath, null, additionalResourceSuffixes);

		assertEquals(4, scanResult.Count);
		string processFileName = "VfsProcessScannerTest.bpmn20.xml";
		assertTrue("'" + processFileName + "' not found", contains(scanResult, processFileName));
		assertTrue("'hello.py' in resource path found", contains(scanResult, "hello.py"));
		assertTrue("'hello.rb' in resource path found", contains(scanResult, "hello.rb"));
		assertTrue("'hello.groovy' in resource path found", contains(scanResult, "hello.groovy"));
	  }

	  private bool contains(IDictionary<string, sbyte[]> scanResult, string suffix)
	  {
		foreach (string @string in scanResult.Keys)
		{
		  if (@string.EndsWith(suffix, StringComparison.Ordinal))
		  {
			return true;
		  }
		}
		return false;
	  }
	}

}