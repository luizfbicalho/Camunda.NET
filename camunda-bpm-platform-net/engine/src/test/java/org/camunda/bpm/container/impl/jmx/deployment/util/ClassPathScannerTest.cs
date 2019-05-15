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
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using ClassPathProcessApplicationScanner = org.camunda.bpm.container.impl.deployment.scanning.ClassPathProcessApplicationScanner;
	using BeforeClass = org.junit.BeforeClass;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using Parameterized = org.junit.runners.Parameterized;
	using Parameters = org.junit.runners.Parameterized.Parameters;


	/// <summary>
	/// @author Falko Menge
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ClassPathScannerTest
	public class ClassPathScannerTest
	{

	  private readonly string url;
	  private static ClassPathProcessApplicationScanner scanner;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameters public static java.util.List<Object[]> data()
	  public static IList<object[]> data()
	  {
		return Arrays.asList(new object[][]
		{
			new object[] {"file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathWithFiles/"},
			new object[] {"file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathWithFilesRecursive/"},
			new object[] {"file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathWithFilesRecursiveTwoDirectories/"},
			new object[] {"file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathWithAdditionalResourceSuffixes/"},
			new object[] {"file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPath.jar"},
			new object[] {"file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathRecursive.jar"},
			new object[] {"file:src/test/resources/org/camunda/bpm/container/impl/jmx/deployment/util/ClassPathScannerTest.testScanClassPathRecursiveTwoDirectories.jar"}
		});
	  }


	  public ClassPathScannerTest(string url)
	  {
		this.url = url;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void setup()
	  public static void setup()
	  {
		scanner = new ClassPathProcessApplicationScanner();
	  }

	  /// <summary>
	  /// Test method for <seealso cref="org.camunda.bpm.container.impl.deployment.scanning.ClassPathProcessApplicationScanner#scanClassPath(java.lang.ClassLoader)"/>. </summary>
	  /// <exception cref="MalformedURLException">  </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanClassPath() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanClassPath()
	  {

		URLClassLoader classLoader = Classloader;

		IDictionary<string, sbyte[]> scanResult = new Dictionary<string, sbyte[]>();

		scanner.scanPaResourceRootPath(classLoader, new URL(url + "/META-INF/processes.xml"), null, scanResult);

		assertTrue("'testDeployProcessArchive.bpmn20.xml' not found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		assertTrue("'testDeployProcessArchive.png' not found", contains(scanResult, "testDeployProcessArchive.png"));
		if (url.Contains("TwoDirectories"))
		{
		  assertEquals(4, scanResult.Count);
		}
		else
		{
		  assertEquals(2, scanResult.Count);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanClassPathWithNonExistingRootPath_relativeToPa() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanClassPathWithNonExistingRootPath_relativeToPa()
	  {

		URLClassLoader classLoader = Classloader;

		IDictionary<string, sbyte[]> scanResult = new Dictionary<string, sbyte[]>();
		scanner.scanPaResourceRootPath(classLoader, new URL(url + "/META-INF/processes.xml"), "pa:nonexisting", scanResult);

		assertFalse("'testDeployProcessArchive.bpmn20.xml' found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		assertFalse("'testDeployProcessArchive.png' found", contains(scanResult, "testDeployProcessArchive.png"));
		assertEquals(0, scanResult.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanClassPathWithNonExistingRootPath_nonRelativeToPa() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanClassPathWithNonExistingRootPath_nonRelativeToPa()
	  {

		URLClassLoader classLoader = Classloader;

		IDictionary<string, sbyte[]> scanResult = new Dictionary<string, sbyte[]>();
		scanner.scanPaResourceRootPath(classLoader, null, "nonexisting", scanResult);

		assertFalse("'testDeployProcessArchive.bpmn20.xml' found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		assertFalse("'testDeployProcessArchive.png' found", contains(scanResult, "testDeployProcessArchive.png"));
		assertEquals(0, scanResult.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanClassPathWithExistingRootPath_relativeToPa() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanClassPathWithExistingRootPath_relativeToPa()
	  {

		URLClassLoader classLoader = Classloader;

		IDictionary<string, sbyte[]> scanResult = new Dictionary<string, sbyte[]>();
		scanner.scanPaResourceRootPath(classLoader, new URL(url + "/META-INF/processes.xml"), "pa:directory/", scanResult);

		if (url.Contains("Recursive"))
		{
		  assertTrue("'testDeployProcessArchive.bpmn20.xml' not found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		  assertTrue("'testDeployProcessArchive.png' not found", contains(scanResult, "testDeployProcessArchive.png"));
		  assertEquals(2, scanResult.Count);
		}
		else
		{
		  assertFalse("'testDeployProcessArchive.bpmn20.xml' found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		  assertFalse("'testDeployProcessArchive.png' found", contains(scanResult, "testDeployProcessArchive.png"));
		  assertEquals(0, scanResult.Count);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanClassPathWithExistingRootPath_nonRelativeToPa() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanClassPathWithExistingRootPath_nonRelativeToPa()
	  {

		URLClassLoader classLoader = Classloader;

		IDictionary<string, sbyte[]> scanResult = new Dictionary<string, sbyte[]>();
		scanner.scanPaResourceRootPath(classLoader, null, "directory/", scanResult);

		if (url.Contains("Recursive"))
		{
		  assertTrue("'testDeployProcessArchive.bpmn20.xml' not found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		  assertTrue("'testDeployProcessArchive.png' not found", contains(scanResult, "testDeployProcessArchive.png"));
		  assertEquals(2, scanResult.Count);
		}
		else
		{
		  assertFalse("'testDeployProcessArchive.bpmn20.xml' found", contains(scanResult, "testDeployProcessArchive.bpmn20.xml"));
		  assertFalse("'testDeployProcessArchive.png' found", contains(scanResult, "testDeployProcessArchive.png"));
		  assertEquals(0, scanResult.Count);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testScanClassPathWithAdditionalResourceSuffixes() throws java.net.MalformedURLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testScanClassPathWithAdditionalResourceSuffixes()
	  {
		URLClassLoader classLoader = Classloader;

		string[] additionalResourceSuffixes = new string[] {"py", "rb", "groovy"};

		IDictionary<string, sbyte[]> scanResult = scanner.findResources(classLoader, null, new URL(url + "/META-INF/processes.xml"), additionalResourceSuffixes);

		if (url.Contains("AdditionalResourceSuffixes"))
		{
		  assertEquals(5, scanResult.Count);
		}
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.net.URLClassLoader getClassloader() throws java.net.MalformedURLException
	  private URLClassLoader Classloader
	  {
		  get
		  {
			return new URLClassLoader(new URL[]{new URL(url)});
		  }
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