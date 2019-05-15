using System;
using System.IO;

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
namespace org.camunda.bpm.integrationtest.util
{
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using ProgrammaticBeanLookup = org.camunda.bpm.engine.cdi.impl.util.ProgrammaticBeanLookup;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Asset = org.jboss.shrinkwrap.api.asset.Asset;
	using ByteArrayAsset = org.jboss.shrinkwrap.api.asset.ByteArrayAsset;
	using Assert = org.junit.Assert;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;


	public abstract class TestHelper
	{

	  public const string PROCESS_XML = "<definitions xmlns=\"http://www.omg.org/spec/BPMN/20100524/MODEL\"  targetNamespace=\"Examples\"><process id=\"PROCESS_KEY\" isExecutable=\"true\" /></definitions>";

	  public static Asset getStringAsAssetWithReplacements(string @string, string[][] replacements)
	  {

		foreach (string[] replacement in replacements)
		{
		  @string = @string.replaceAll(replacement[0], replacement[1]);
		}

		return new ByteArrayAsset(@string.GetBytes());

	  }

	  public static Asset[] generateProcessAssets(int amount)
	  {

		Asset[] result = new Asset[amount];

		for (int i = 0; i < result.Length; i++)
		{
		  result[i] = getStringAsAssetWithReplacements(PROCESS_XML, new string[][]
		  {
			  new string[]{"PROCESS_KEY", "process-" + i}
		  });
		}

		return result;

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void assertDiagramIsDeployed(boolean deployed, Class clazz, String expectedDiagramResource, String processDefinitionKey) throws java.io.IOException
	  public static void assertDiagramIsDeployed(bool deployed, Type clazz, string expectedDiagramResource, string processDefinitionKey)
	  {
		ProcessEngine processEngine = ProgrammaticBeanLookup.lookup(typeof(ProcessEngine));
		Assert.assertNotNull(processEngine);
		RepositoryService repositoryService = processEngine.RepositoryService;
		ProcessDefinition processDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).singleResult();
		assertNotNull(processDefinition);

		Stream actualStream = null;
		Stream expectedStream = null;
		try
		{
		  actualStream = repositoryService.getProcessDiagram(processDefinition.Id);

		  if (deployed)
		  {
			sbyte[] actualDiagram = IoUtil.readInputStream(actualStream, "actualStream");
			assertNotNull(actualDiagram);
			assertTrue(actualDiagram.Length > 0);

			expectedStream = clazz.getResourceAsStream(expectedDiagramResource);
			sbyte[] expectedDiagram = IoUtil.readInputStream(expectedStream, "expectedSteam");
			assertNotNull(expectedDiagram);

			assertTrue(isEqual(expectedStream, actualStream));
		  }
		  else
		  {
			assertNull(actualStream);
		  }
		}
		finally
		{
		  IoUtil.closeSilently(actualStream);
		  IoUtil.closeSilently(expectedStream);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected static boolean isEqual(java.io.InputStream stream1, java.io.InputStream stream2) throws java.io.IOException
	  protected internal static bool isEqual(Stream stream1, Stream stream2)
	  {

		  ReadableByteChannel channel1 = Channels.newChannel(stream1);
		  ReadableByteChannel channel2 = Channels.newChannel(stream2);

		  ByteBuffer buffer1 = ByteBuffer.allocateDirect(1024);
		  ByteBuffer buffer2 = ByteBuffer.allocateDirect(1024);

		  try
		  {
			  while (true)
			  {

				  int bytesReadFromStream1 = channel1.read(buffer1);
				  int bytesReadFromStream2 = channel2.read(buffer2);

				  if (bytesReadFromStream1 == -1 || bytesReadFromStream2 == -1)
				  {
					  return bytesReadFromStream1 == bytesReadFromStream2;
				  }

				  buffer1.flip();
				  buffer2.flip();

				  for (int i = 0; i < Math.Min(bytesReadFromStream1, bytesReadFromStream2); i++)
				  {
					  if (buffer1.get() != buffer2.get())
					  {
						  return false;
					  }
				  }

				  buffer1.compact();
				  buffer2.compact();
			  }

		  }
		  finally
		  {
			  if (stream1 != null)
			  {
				  stream1.Close();
			  }
			  if (stream2 != null)
			  {
				  stream2.Close();
			  }
		  }
	  }

	}

}