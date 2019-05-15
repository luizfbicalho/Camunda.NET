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
namespace org.camunda.bpm.container.impl.deployment.scanning
{

	using ProcessArchiveXml = org.camunda.bpm.application.impl.metadata.spi.ProcessArchiveXml;
	using ProcessApplicationScanner = org.camunda.bpm.container.impl.deployment.scanning.spi.ProcessApplicationScanner;
	using BpmnDeployer = org.camunda.bpm.engine.impl.bpmn.deployer.BpmnDeployer;
	using CmmnDeployer = org.camunda.bpm.engine.impl.cmmn.deployer.CmmnDeployer;
	using DecisionDefinitionDeployer = org.camunda.bpm.engine.impl.dmn.deployer.DecisionDefinitionDeployer;

	public class ProcessApplicationScanningUtil
	{

	  /// 
	  /// <param name="classLoader">
	  ///          the classloader to scan </param>
	  /// <param name="paResourceRootPath">
	  ///          see <seealso cref="ProcessArchiveXml#PROP_RESOURCE_ROOT_PATH"/> </param>
	  /// <param name="metaFileUrl">
	  ///          the URL to the META-INF/processes.xml file </param>
	  /// <returns> a Map of process definitions </returns>
	  public static IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string paResourceRootPath, URL metaFileUrl)
	  {
		return findResources(classLoader, paResourceRootPath, metaFileUrl, null);
	  }

	  /// 
	  /// <param name="classLoader">
	  ///          the classloader to scan </param>
	  /// <param name="paResourceRootPath">
	  ///          see <seealso cref="ProcessArchiveXml#PROP_RESOURCE_ROOT_PATH"/> </param>
	  /// <param name="metaFileUrl">
	  ///          the URL to the META-INF/processes.xml file </param>
	  /// <param name="additionalResourceSuffixes">
	  ///          a list of additional suffixes for resources </param>
	  /// <returns> a Map of process definitions </returns>
	  public static IDictionary<string, sbyte[]> findResources(ClassLoader classLoader, string paResourceRootPath, URL metaFileUrl, string[] additionalResourceSuffixes)
	  {
		ProcessApplicationScanner scanner = null;

		try
		{
		  // check if we must use JBoss VFS
		  classLoader.loadClass("org.jboss.vfs.VFS");
		  scanner = new VfsProcessApplicationScanner();
		}
		catch (Exception)
		{
		  scanner = new ClassPathProcessApplicationScanner();
		}

		return scanner.findResources(classLoader, paResourceRootPath, metaFileUrl, additionalResourceSuffixes);

	  }

	  public static bool isDeployable(string filename)
	  {
		return hasSuffix(filename, BpmnDeployer.BPMN_RESOURCE_SUFFIXES) || hasSuffix(filename, CmmnDeployer.CMMN_RESOURCE_SUFFIXES) || hasSuffix(filename, DecisionDefinitionDeployer.DMN_RESOURCE_SUFFIXES);
	  }

	  public static bool isDeployable(string filename, string[] additionalResourceSuffixes)
	  {
		return isDeployable(filename) || hasSuffix(filename, additionalResourceSuffixes);
	  }

	  public static bool hasSuffix(string filename, string[] suffixes)
	  {
		if (suffixes == null || suffixes.Length == 0)
		{
		  return false;
		}
		else
		{
		  foreach (string suffix in suffixes)
		  {
			if (filename.EndsWith(suffix, StringComparison.Ordinal))
			{
			  return true;
			}
		  }
		  return false;
		}
	  }

	  public static bool isDiagram(string fileName, string modelFileName)
	  {
		// process resources
		bool isBpmnDiagram = checkDiagram(fileName, modelFileName, BpmnDeployer.DIAGRAM_SUFFIXES, BpmnDeployer.BPMN_RESOURCE_SUFFIXES);
		// case resources
		bool isCmmnDiagram = checkDiagram(fileName, modelFileName, CmmnDeployer.DIAGRAM_SUFFIXES, CmmnDeployer.CMMN_RESOURCE_SUFFIXES);
		// decision resources
		bool isDmnDiagram = checkDiagram(fileName, modelFileName, DecisionDefinitionDeployer.DIAGRAM_SUFFIXES, DecisionDefinitionDeployer.DMN_RESOURCE_SUFFIXES);

		return isBpmnDiagram || isCmmnDiagram || isDmnDiagram;
	  }

	  /// <summary>
	  /// Checks, whether a filename is a diagram for the given modelFileName.
	  /// </summary>
	  /// <param name="fileName"> filename to check. </param>
	  /// <param name="modelFileName"> model file name. </param>
	  /// <param name="diagramSuffixes"> suffixes of the diagram files. </param>
	  /// <param name="modelSuffixes"> suffixes of model files. </param>
	  /// <returns> true, if a file is a diagram for the model. </returns>
	  protected internal static bool checkDiagram(string fileName, string modelFileName, string[] diagramSuffixes, string[] modelSuffixes)
	  {
		foreach (string modelSuffix in modelSuffixes)
		{
		  if (modelFileName.EndsWith(modelSuffix, StringComparison.Ordinal))
		  {
			string caseFilePrefix = modelFileName.Substring(0, modelFileName.Length - modelSuffix.Length);
			if (fileName.StartsWith(caseFilePrefix, StringComparison.Ordinal))
			{
			  foreach (string diagramResourceSuffix in diagramSuffixes)
			  {
				if (fileName.EndsWith(diagramResourceSuffix, StringComparison.Ordinal))
				{
				  return true;
				}
			  }
			}
		  }
		}
		return false;
	  }
	}

}