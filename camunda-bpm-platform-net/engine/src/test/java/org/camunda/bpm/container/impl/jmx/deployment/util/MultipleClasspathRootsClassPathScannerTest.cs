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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using ClassPathProcessApplicationScanner = org.camunda.bpm.container.impl.deployment.scanning.ClassPathProcessApplicationScanner;
	using Test = org.junit.Test;


	/// <summary>
	/// @author Falko Menge
	/// @author Daniel Meyer
	/// </summary>
	public class MultipleClasspathRootsClassPathScannerTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanClassPath_multipleRoots() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanClassPath_multipleRoots()
	  {

		// define a classloader with multiple roots.
		URLClassLoader classLoader = new URLClassLoader(new URL[]
		{
			new URL("file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathWithFiles/"),
			new URL("file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathWithFilesRecursive/"),
			new URL("file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathRecursiveTwoDirectories.jar")
		});

		ClassPathProcessApplicationScanner scanner = new ClassPathProcessApplicationScanner();

		IDictionary<string, sbyte[]> scanResult = new Dictionary<string, sbyte[]>();

		scanner.scanPaResourceRootPath(classLoader, null, "classpath:directory/",scanResult);

		assertTrue("'testDeployProcessArchive.bpmn20.xml' not found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		assertTrue("'testDeployProcessArchive.png' not found", contains(scanResult, "testDeployProcessArchive.png"));
		assertEquals(2, scanResult.Count); // only finds two files since the resource name of the processes (and diagrams) is the same

		scanResult.Clear();
		scanner.scanPaResourceRootPath(classLoader, null, "directory/", scanResult);

		assertTrue("'testDeployProcessArchive.bpmn20.xml' not found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		assertTrue("'testDeployProcessArchive.png' not found", contains(scanResult, "testDeployProcessArchive.png"));
		assertEquals(2, scanResult.Count); // only finds two files since the resource name of the processes (and diagrams) is the same

		scanResult.Clear();
		scanner.scanPaResourceRootPath(classLoader, new URL("file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathWithFilesRecursive/META-INF/processes.xml"), "pa:directory/", scanResult);

		assertTrue("'testDeployProcessArchive.bpmn20.xml' not found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		assertTrue("'testDeployProcessArchive.png' not found", contains(scanResult, "testDeployProcessArchive.png"));
		assertEquals(2, scanResult.Count); // only finds two files since a PA-local resource root path is provided

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